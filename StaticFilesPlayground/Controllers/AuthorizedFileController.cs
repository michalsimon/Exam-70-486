namespace StaticFilesPlayground.Controllers
{
    using System.IO;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("files")]
    public class AuthorizedFileController: Controller
    {
        [Authorize]
        [Route("banner")]
        public IActionResult BannerImage()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), 
                "MyStaticFiles", "images", "banner1.svg");

            return PhysicalFile(file, "image/svg+xml");
        }
    }
}