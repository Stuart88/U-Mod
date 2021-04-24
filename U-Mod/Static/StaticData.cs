using System;
using System.IO;
using System.Linq;
using U_Mod.Games.Oblivion.Models;
using U_Mod.Models;
using AmgShared.Models;
using AMGWebsite.Shared.Models;
using AMGWebsite.Shared.Enums;

namespace U_Mod.Static
{
    public static class StaticData
    {
        

        public static MasterList MasterList = new MasterList();
        public static KillswitchInfo KillswitchInfo = new KillswitchInfo();
        public static SoftwareVersion InstallerInfo = new SoftwareVersion();
        public static SystemInfo SystemInfo = new SystemInfo();
        public static UserDataStore UserDataStore = new UserDataStore();
        public static AppData AppData = new AppData();
        public static bool IsLoggedIn = false;
        

        /// <summary>
        /// The game that the installer is currently on. Assign when in MainWindow of each installer
        /// </summary>
        public static GamesEnum CurrentGame { get; set; }


        /// <summary>
        /// Saves main app data and local app data for the currently selected game
        /// </summary>
        public static void SaveAppData(bool isReinstall = false)
        {
            Helpers.FileHelpers.SaveAsJson(AppData, Constants.AppDataFilePath);// Save to game hub install folder

            UserDataBase toSave = null;
            string savePath = "";

            switch (CurrentGame)
            {
                case GamesEnum.Oblivion:
                    toSave = UserDataStore.OblivionUserData;
#if DEV
                    savePath = Constants.OblivionDataFileName;
#else
                    savePath = Path.Combine(AfterMarketGamesFolder, Constants.OblivionDataFileName);
#endif
                    CompleteSave<Games.Oblivion.Models.UserData>();
                    break;

                case GamesEnum.Fallout:
                    toSave = UserDataStore.FalloutUserData;
#if DEV
                    savePath = Constants.FalloutDataFileName;
#else
                    savePath = Path.Combine(AfterMarketGamesFolder, Constants.FalloutDataFileName);
#endif
                    CompleteSave<Games.Fallout.Models.UserData>();
                    break;
            }

            // Save path is null if SaveAppData is called before user has selected their game folder location.
            void CompleteSave<T>() where T : UserDataBase
            {
                if (toSave != null && (!string.IsNullOrEmpty(AfterMarketGamesFolder)) || isReinstall)
                {
                    Helpers.FileHelpers.SaveAsJson<T>(toSave as T, savePath);
                }
            }
        }

        

        public static GameItem GetCurrentGame()
        {
            return MasterList.Games.First(g => g.Game == Static.StaticData.CurrentGame);
        }


        private static string AfterMarketGamesFolder => Helpers.FileHelpers.GetAfterMarketGamesFolder();

        /// <summary>
        /// Loads local app data that has been saved to user's device
        /// </summary>
        public static void LoadAppData()
        {
            if (!Directory.Exists(Constants.AppDataFolder))
                Directory.CreateDirectory(Constants.AppDataFolder);

            if (File.Exists(Constants.AppDataFilePath))
                AppData = Helpers.FileHelpers.LoadFile<AppData>(Constants.AppDataFilePath);
            else
            {
                AppData = new AppData();
                SaveAppData();
            }
        }
        /// <summary>
        /// Finds and loads the AmgData.json file in the 
        /// </summary>
        public static void LoadGameData()
        {
            switch (CurrentGame)
            {
                case GamesEnum.Oblivion:
#if DEV
                    UserDataStore.OblivionUserData = Helpers.FileHelpers.LoadFile<U_Mod.Games.Oblivion.Models.UserData>(Constants.OblivionDataFileName);
#else
                    UserDataStore.OblivionUserData = Helpers.FileHelpers.LoadFile<U_Mod.Games.Oblivion.Models.UserData>(Path.Combine(AfterMarketGamesFolder, Constants.OblivionDataFileName));
#endif
                    break;

                case GamesEnum.Fallout:
#if DEV
                    UserDataStore.FalloutUserData = Helpers.FileHelpers.LoadFile<U_Mod.Games.Fallout.Models.UserData>(Constants.FalloutDataFileName);
#else
                    UserDataStore.FalloutUserData = Helpers.FileHelpers.LoadFile<U_Mod.Games.Fallout.Models.UserData>(Path.Combine(AfterMarketGamesFolder, Constants.FalloutDataFileName));
#endif
                    break;
            }
        }

        public static string GetCurrentGameDataFileName()
        {
            return CurrentGame switch
            {
                GamesEnum.Oblivion => Constants.OblivionDataFileName,
                GamesEnum.Fallout => Constants.FalloutDataFileName,
            };
        }
    }
}
