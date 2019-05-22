using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OdeToFood.Areas.Identity.Data;
using OdeToFood.Models;

[assembly: HostingStartup(typeof(OdeToFood.Areas.Identity.IdentityHostingStartup))]
namespace OdeToFood.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<OdeToFoodIdentityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("OdeToFoodDb")));

                services.AddDefaultIdentity<OdeToFoodUser>()
                    .AddEntityFrameworkStores<OdeToFoodIdentityContext>();
            });
        }
    }
}