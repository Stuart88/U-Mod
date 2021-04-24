using U_Mod.Pages.BaseClasses;

namespace U_Mod.Models
{
    public class ModProcessingTracker
    {
        public ModListItem ModListItem { get; set; }
        public bool IsManualDownload { get; set; }
        public bool IsDirectDownload { get; set; }
        public bool IsAutoDownload { get; set; }
        public string ManualDownloadUrl { get; set; }

        /// <summary>
        /// The path the file was downloaded to
        /// </summary>
        public string DownloadedPath { get; set; }
        public bool IsDownloaded { get; set; }
        public bool IsUnzipped { get; set; }
        public bool IsTransferred { get; set; }
    }
}
