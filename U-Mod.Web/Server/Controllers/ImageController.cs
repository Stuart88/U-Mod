using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;



namespace U_Mod.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ImageController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

 
        [AllowAnonymous]
        [HttpGet("[action]/{path}")]
        public IActionResult GetImage(string path)
        {
            path = System.Web.HttpUtility.UrlDecode(path);

            string mimeType;
            
            if (path.EndsWith(".svg"))
            {
                mimeType = "image/svg+xml";
            }
            else if (path.EndsWith(".gif"))
            {
                mimeType = "image/gif";
            }
            else
            {
                mimeType = "image/png";
            }

            
            return File($"/images/{path}", mimeType, $"{path}");
        }

        [AllowAnonymous]
        [HttpGet("[action]/{path}")]
        public IActionResult GetImageCompressed(string path)
        {

            path = System.Web.HttpUtility.UrlDecode(path);

            string mimeType;

            if (path.EndsWith(".svg"))
            {
                mimeType = "image/svg+xml";
            }
            else if (path.EndsWith(".gif"))
            {
                mimeType = "image/gif";
            }
            else
            {
                mimeType = "image/png";
            }

            return File($"/images-compressed/{path}", mimeType, $"{path}");
        }
    }
}
