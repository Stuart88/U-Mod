using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace U_Mod.Client.Helpers
{
    public static class ServerHelpers
    {
        public static string ImageSrcString(string filePath, bool compress = false)
        {
            return compress
                ? $"Image/GetImageCompressed/{System.Web.HttpUtility.UrlEncode(filePath)}"
                : $"Image/GetImage/{System.Web.HttpUtility.UrlEncode(filePath)}";
        }

        public static string GetFileUrl(string fileName)
        {
            return $"Download/GetFile/{System.Web.HttpUtility.UrlEncode(fileName)}";
        }
    }
}
