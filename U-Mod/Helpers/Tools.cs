﻿using AMGWebsite.Shared.Enums;
using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;

namespace U_Mod.Helpers
{
    public class Tools
    {
        #region Public Methods

        public static void CreateShortcut(string shortcutDescription, string shortcutPath, string targetFileLocation)
        {
            string shortcutLocation = shortcutPath + ".lnk";
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = shortcutDescription;   // The description of the shortcut
            //shortcut.IconLocation = @"c:\myicon.ico";           // The icon of the shortcut
            shortcut.TargetPath = targetFileLocation;                 // The path of the file that will launch when the shortcut is run
            shortcut.Save();                                    // Save the shortcut
        }

        public static void InstallUpdate(string updateLocation)
        {
            LaunchProcess(updateLocation, "InstallUpdate");
        }

        public static void LaunchGame()
        {
            string exeName = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => GeneralHelpers.GetUserDataForGame().IsSteamGame
                ? "OblivionLauncher.exe"
                : "obse_loader.exe",
                GamesEnum.Fallout => GeneralHelpers.GetUserDataForGame().IsSteamGame
                //? "fose_loader.exe.shortcut.lnk"
                //: "fose_loader.exe.shortcut.lnk",
                ? "fose_loader.exe"
                : "fose_loader.exe",
                GamesEnum.NewVegas => throw new NotImplementedException(),
                GamesEnum.Unknown => throw new NotImplementedException()
            };

            LaunchProcessInGameFolder(exeName, $"LaunchGame: {GeneralHelpers.GetGameName()}");
        }

        public static void LaunchModManager()
        {
            string exeName = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => "OblivionModManager.exe",
                GamesEnum.Fallout => Path.Combine("GeMM", "fomm.exe")
            };

            LaunchProcessInGameFolder(exeName, $"RunModManager: {GeneralHelpers.GetGameName()}");
        }

        public static void Run4GbPatch(bool withArgs = true)
        {
            (string exeName, string args) data = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => ("4gb patch.exe", "Oblivion.exe"),
                GamesEnum.Fallout => ("4gb_patch.exe", "Fallout3.exe")
            };

            var args = withArgs ? data.args : "";
            
            LaunchProcessInGameFolder(data.exeName, $"Run4GbPatch: {GeneralHelpers.GetGameName()}", args);
        }

        public static bool ModManagerInstalled()
        {
            string filePath = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => "OblivionModManager.exe",
                GamesEnum.Fallout => Path.Combine("GeMM", "fomm.exe")
            };

            return System.IO.File.Exists(System.IO.Path.Combine(FileHelpers.GetGameFolder(), filePath));
        }

        public static void RunModManagerSetup()
        {
            string exeName = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => "obmm_setup.exe",
                GamesEnum.Fallout => "FOMM-36901-0-13-21.exe"
            };

            LaunchProcessInGameFolder(exeName, $"RunModManagerSetup: {GeneralHelpers.GetGameName()}");
        }

        #endregion Public Methods

        #region Private Methods

        private static void LaunchProcessInGameFolder(string exeName, string exceptionTitle, string arguments = "")
        {
            try
            {
                string fileName = Path.Combine(FileHelpers.GetGameFolder(), exeName);
                Process p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        UseShellExecute = true,
                        WorkingDirectory = FileHelpers.GetGameFolder(),
                        Arguments = arguments
                    }
                };

                p.Start();
            }
            catch (Exception e)
            {
                GeneralHelpers.ShowExceptionMessageBox(e);
                Logging.Logger.LogException(exceptionTitle, e);
            }
        }

        private static void LaunchProcess(string processPath, string exceptionTitle, string arguments = "")
        {
            try
            {
                Process p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = processPath,
                        UseShellExecute = true,
                        Arguments = arguments
                    }
                };

                p.Start();
            }
            catch (Exception e)
            {
                GeneralHelpers.ShowExceptionMessageBox(e);
                Logging.Logger.LogException(exceptionTitle, e);
            }
        }



        #endregion Private Methods
    }
}