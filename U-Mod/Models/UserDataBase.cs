using System;
using System.Collections.Generic;
using U_Mod.Pages.BaseClasses;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Models;

namespace U_Mod.Models
{
    public class UserDataBase
    {
        public bool IgnoredCDrivecheck { get; set; }
        public bool IsSteamGame { get; set; }
        public bool HasAllDlc { get; set; }
        public bool CanFullInstall { get; set; } = true; // Leftover from Levi installer. In U-Mod, all users can full install
        public List<ModVersionInfo> InstalledModIds { get; set; } = new List<ModVersionInfo>();
        public List<ModVersionInfo> ByPassedModIds { get; set; } = new List<ModVersionInfo>();
        public bool InstallationComplete { get; set; }
        public string GameVersion { get; set; }

        /// <summary>
        /// Mods Selected for download on the mods list
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<ModListItem> SelectedToInstall { get; set; } = new List<ModListItem>();

        /// <summary>
        /// Set this to allow bypass of base install procedures which have already been done
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsUpdating { get; set; }

        public InstallProfileEnum InstallProfile =>
            (this.HasAllDlc ? InstallProfileEnum.AllDlc : InstallProfileEnum.NoDlc) |
            (this.IsSteamGame ? InstallProfileEnum.Steam : InstallProfileEnum.NonSteam);

        public bool On4GbRamPatch { get; set; }
        public bool OnModManagerPage { get; set; }
        public bool OnFinalPage { get; set; }
    }
}
