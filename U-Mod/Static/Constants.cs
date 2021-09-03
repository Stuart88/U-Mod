using System;
using System.IO;

namespace U_Mod.Static
{
    public static class Constants
    {
        #region Public Fields

        public const string UMod = "U-Mod";
        public const string UModBackup = "U-ModBackup.zip";
        public const  string AppDataFileName = "AppData.json";
        public const string MasterListLink = "https://u-mod.club/Download/GetFile/masterList.json";
        public const string SoftwareVersionLink = "https://u-mod.club/Download/GetFile/softwareVersion.json";
        public const string DefaultSoftwareVersion = "-1";
        public const string UpdateUrl = "https://u-mod.club/Download/GetFile/U_Mod-Setup.exe";
        public const string WebsiteUrl = "https://u-mod.club/";
        public const string OblivionDataFileName = "OblivionModInstallData.json";
        public const string FalloutDataFileName = "FalloutModInstallData.json";
        public const string NewVegasDataFileName = "NewVegasModInstallData.json";
        public static string AppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "U_Mod");
        public static string AppDataFilePath = Path.Combine(AppDataFolder, AppDataFileName);

#if DEBUG || DEV
        public const string NexusApiKey = "Q0xCME9uNTlUUzd3SUNMbTU2UjlCdHNMeFdHNHZHa1hmWVZ6c1FSZkdtdVhUOEt0SWJOcjNVL295djhRZ2tWWS0tbU5hZnBhd3ZEY04wSDJuNURZWnRMdz09--569d3193df5f017d17a46e085b69e5d16df1c196";
#endif

        #endregion Public Fields

        public const string ModOrganizer2 = "Mod Organizer 2";
        public const string OblivionModManager = "Oblivion Mod Manager";
        public const string Obmm = "OBMM";
    }
}