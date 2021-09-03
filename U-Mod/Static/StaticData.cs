using System;
using System.IO;
using System.Linq;
using U_Mod.Models;
using U_Mod.Shared.Models;
using U_Mod.Shared.Enums;

namespace U_Mod.Static
{
    public static class StaticData
    {
        

        public static MasterList MasterList = new MasterList();
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

            UserDataBase toSave;
            string savePath;

            switch (CurrentGame)
            {
                case GamesEnum.Oblivion:
                    toSave = UserDataStore.OblivionUserData;
                    savePath = Path.Combine(UModFolder, Constants.OblivionDataFileName);
                    CompleteSave<UserDataBase>();
                    break;

                case GamesEnum.Fallout:
                    toSave = UserDataStore.FalloutUserData;
                    savePath = Path.Combine(UModFolder, Constants.FalloutDataFileName);
                    CompleteSave<UserDataBase>();
                    break;

                case GamesEnum.NewVegas:
                    toSave = UserDataStore.NewVegasUserData;
                    savePath = Path.Combine(UModFolder, Constants.NewVegasDataFileName);
                    CompleteSave<UserDataBase>();
                    break;
            }

            // Save path is null if SaveAppData is called before user has selected their game folder location.
            void CompleteSave<T>() where T : UserDataBase
            {
                if (toSave != null && (!string.IsNullOrEmpty(UModFolder)) || isReinstall)
                {
                    Helpers.FileHelpers.SaveAsJson<T>(toSave as T, savePath);
                }
            }
        }

        

        public static GameItem GetCurrentGame()
        {
            return MasterList.Games.First(g => g.Game == Static.StaticData.CurrentGame);
        }


        private static string UModFolder => Helpers.FileHelpers.GetUModFolder();

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
                    UserDataStore.OblivionUserData = Helpers.FileHelpers.LoadFile<UserDataBase>(Path.Combine(UModFolder, Constants.OblivionDataFileName));
                    break;

                case GamesEnum.Fallout:
                    UserDataStore.FalloutUserData = Helpers.FileHelpers.LoadFile<UserDataBase>(Path.Combine(UModFolder, Constants.FalloutDataFileName));
                    break;

                case GamesEnum.NewVegas:
                    UserDataStore.NewVegasUserData = Helpers.FileHelpers.LoadFile<UserDataBase>(Path.Combine(UModFolder, Constants.NewVegasDataFileName));
                    break;
            }
        }

        public static string GetCurrentGameDataFileName()
        {
            return CurrentGame switch
            {
                GamesEnum.Oblivion => Constants.OblivionDataFileName,
                GamesEnum.Fallout => Constants.FalloutDataFileName,
                GamesEnum.NewVegas => Constants.NewVegasDataFileName,
            };
        }
    }
}
