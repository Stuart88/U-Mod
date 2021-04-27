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
    public class DownloadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public DownloadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        [Route("[action]/{filename}")]
        public IActionResult GetFile([FromRoute] string filename)
        {
            return File($"downloads/{filename}", System.Net.Mime.MediaTypeNames.Application.Octet);
        }
    }
}
