using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
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
    public class ModController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public ModController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        [Route("[action]")]
        public BasicHttpResponse<MasterList> Masterlist()
        {
            var masterList = JsonSerializer.Deserialize<MasterList>(System.IO.File.ReadAllText("wwwroot/downloads/masterList.json"));

            return new BasicHttpResponse<MasterList>
            {
                Data = masterList,
                Ok = true
            };
        }
    }
}
