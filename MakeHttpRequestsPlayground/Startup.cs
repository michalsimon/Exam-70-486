namespace MakeHttpRequestsPlayground
{
    using System;
    using System.Net.Http;
    using MakeHttpRequestsPlayground.GitHub.Services;
    using MakeHttpRequestsPlayground.Handlers;
    using MakeHttpRequestsPlayground.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Polly;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // basic usage
            services.AddHttpClient();

            // named client
            services.AddHttpClient(
                "github",
                c =>
                {
                    c.BaseAddress = new Uri("https://api.github.com/");
                    // Github API versioning
                    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    // Github requires a user-agent
                    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
                });

            // typed client where configuration occurs in ctor
            services.AddHttpClient<GitHubService>();

            // typed client where configuration occurs during registration
            services.AddHttpClient<RepoService>(
                c =>
                {
                    c.BaseAddress = new Uri("https://api.github.com/");
                    c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                    c.DefaultRequestHeaders.Add("User-Agent", "HttpClientFactory-Sample");
                });

            #region handlers

            // registration of handler
            services.AddTransient<ValidateHeaderHandler>();

            services.AddHttpClient(
                    "externalservice",
                    c =>
                    {
                        // Assume this is an "external" service which requires an API KEY
                        c.BaseAddress = new Uri("https://localhost:5000/");
                    })
                .AddHttpMessageHandler<ValidateHeaderHandler>();

            // register multiple handlers
            services.AddTransient<SecureRequestHandler>();
            services.AddTransient<RequestDataHandler>();

            services.AddHttpClient("clientwithhandlers")
                // This handler is on the outside and called first during the 
                // request, last during the response.
                .AddHttpMessageHandler<SecureRequestHandler>()
                // This handler is on the inside, closest to the request being 
                // sent.
                .AddHttpMessageHandler<RequestDataHandler>();

            #endregion

            #region polly policies

            services.AddHttpClient<UnreliableEndpointCallerService>()
                .AddTransientHttpErrorPolicy(
                    p =>
                        p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

            var timeout = Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(10));
            var longTimeout = Policy.TimeoutAsync<HttpResponseMessage>(
                TimeSpan.FromSeconds(30));

            services.AddHttpClient("conditionalpolicy")
                // Run some code to select a policy based on the request
                .AddPolicyHandler(
                    request =>
                        request.Method == HttpMethod.Get ? timeout : longTimeout);

            services.AddHttpClient("multiplepolicies")
                .AddTransientHttpErrorPolicy(p => p.RetryAsync(3))
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            var registry = services.AddPolicyRegistry();

            registry.Add("regular", timeout);
            registry.Add("long", longTimeout);

            services.AddHttpClient("regulartimeouthandler")
                .AddPolicyHandlerFromRegistry("regular");

            #endregion

            // override HttpMessageHandler handler lifetime (default 2 min.)
            services.AddHttpClient("extendedhandlerlifetime")
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            services.AddHttpClient("configured-inner-handler")
                .ConfigurePrimaryHttpMessageHandler(
                    () =>
                    {
                        return new HttpClientHandler()
                        {
                            AllowAutoRedirect = false,
                            UseDefaultCredentials = true
                        };
                    });

            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.None;
                });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}