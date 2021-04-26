using System;
using System.Collections.Generic;
using System.Text;

namespace U_Mod.Shared.Helpers
{
    public static class Helpers
    {
        public static string ImageSrcString(string filePath, bool compress = false)
        {
            if (string.IsNullOrEmpty(filePath))
                return "";

            return compress
                ? $"image/compressed/{System.Web.HttpUtility.UrlEncode(filePath)}"
                : $"image/{System.Web.HttpUtility.UrlEncode(filePath)}";
        }
    }
}
