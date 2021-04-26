
using U_Mod.Shared.Models;

namespace U_Mod.Models
{
    public class DirectDownloadData
    {
        //public Mod Mod { get; set; }
        public ModZipFile ZipFile { get; set; }
        public string DirectDownloadUrl { get; set; }
        public string FileName { get; set; }
        public string DownloadedPath { get; set; }
        public bool IsAutoDownload { get; set; }
    }
}
