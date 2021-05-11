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
using U_Mod.Shared.Models;

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
            try
            {
                if (!System.IO.File.Exists(Path.Combine(_environment.WebRootPath,"downloads",filename)))
                    return new JsonResult("File not found!");

                return File($"downloads/{filename}", System.Net.Mime.MediaTypeNames.Application.Octet);
            }
            catch(Exception e)
            {
                return (IActionResult)e;
            }
        }

        // POST
        [HttpPost]
        [AcceptVerbs("POST", "post")]
        [Route("[action]")]
        public async Task<BasicHttpResponse<bool>> UploadMasterList([FromBody]string masterlistJson)
        {

            if (!string.IsNullOrEmpty(masterlistJson))
            {
                try
                {
                    MasterList masterList = System.Text.Json.JsonSerializer.Deserialize<MasterList>(masterlistJson);

                    if (masterList == null || masterList.Games.Count == 0)
                        throw new Exception("Could not parse masterlist data!");

                    string currentDir = Environment.CurrentDirectory;
                    string appPath = Path.Combine(currentDir, "wwwroot", "downloads");

                    U_Mod.Shared.Helpers.FileHelpers.SaveAsJson<MasterList>(masterList, Path.Combine(appPath, "masterList.json"));
                   
                    return new BasicHttpResponse<bool>
                    {
                        Data = true,
                        Ok = true
                    };
                }
                catch (Exception e)
                {
                    return new BasicHttpResponse<bool>
                    {
                        Data = true,
                        Ok = false,
                        Message = e.Message
                    };
                }
            }
            else
            {
                return new BasicHttpResponse<bool>
                {
                    Data = false,
                    Ok = false,
                    Message = "No files uploaded!"
                };
            }
        }
    }
}
