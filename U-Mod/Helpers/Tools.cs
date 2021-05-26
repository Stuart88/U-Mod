﻿using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using U_Mod.Shared.Enums;

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
                ? "fose_loader.exe"
                : "fose_loader.exe",
                GamesEnum.NewVegas => throw new NotImplementedException(),
                GamesEnum.Unknown => throw new NotImplementedException()
            };

            LaunchProcessInGameFolder(exeName, $"LaunchGame: {GeneralHelpers.GetGameName()}");
        }

        public static void LaunchModManager()
        {
            (string exeName, string args) = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => ("OblivionModManager.exe", ""),
                GamesEnum.Fallout => (Path.Combine("Mod Organizer 2-6194-2-4-2-1620741202", "Mod Organizer 2-6194-2-4-2-1620741202.exe"), "")
            };

            LaunchProcessInGameFolder(exeName, $"RunModManager: {GeneralHelpers.GetGameName()}", args);
        }

        public static void OpenModOrganizerHelpInBrowser()
        {
            string urlPath = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Fallout => "games/fallout-3",
                GamesEnum.Oblivion => "games/oblivion",
                _ => ""
            };
            ProcessHelpers.OpenInBrowser(Static.Constants.WebsiteUrl + urlPath);
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

        public static bool IsProgramInstalled(string programDisplayName)
        {
            foreach (var item in Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall").GetSubKeyNames())
            {

                object programName = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\" + item).GetValue("DisplayName");

                Console.WriteLine(programName);
                if (string.Equals(programName, programDisplayName))
                {
                    return true;
                }
            }
            return false;
        }



        #endregion Private Methods
    }
}