using U_Mod.Helpers;
using U_Mod.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using U_Mod.Shared.Models;
using U_Mod.Pages.BaseClasses;
using System.IO;

namespace U_Mod.Extensions
{
    public static class MasterListExtensions
    {
        #region Public Methods

        public static List<Mod> GetAvailableModsList(this MasterList m, bool includeEssential, bool includeInstalled = false)
        {
            UserDataBase user = Static.StaticData.UserDataStore.CurrentUserData;

            string gameName = Helpers.GeneralHelpers.GetGameName();

            return m.Games
                       .FirstOrDefault(x => x.GameName == gameName)?
                       .Mods
                       .OrderBy(mod => mod.ModName)
                       .Where(moddy => (!includeEssential && !moddy.IsEssential || includeEssential)
                                   && (!includeInstalled && !ModHelpers.IsInstalled(moddy) || includeInstalled)
                                   && ModHelpers.UserCanInstall(moddy))
                       .ToList()
                   ?? new List<Mod>();
        }

        public static List<Mod> GetEssentialModsList(this MasterList m, bool includeInstalled = false)
        {
            UserDataBase user = Static.StaticData.UserDataStore.CurrentUserData;

            string gameName = Helpers.GeneralHelpers.GetGameName();

            return m.Games
                       .FirstOrDefault(x => x.GameName == gameName)?
                       .Mods
                       .OrderBy(mod => mod.ModName)
                       .Where(moddy => (moddy.IsEssential)
                                       //&& user.SelectedToInstall.All(mod => mod.Mod.Id != moddy.Id)
                                       && ((!includeInstalled && !ModHelpers.IsInstalled(moddy) || includeInstalled)
                                           && ModHelpers.UserCanInstall(moddy)))
                       .ToList()
                   ?? new List<Mod>();
        }



       /// <summary>
       /// Returns entire mods list for the current game
       /// </summary>
       /// <param name="m"></param>
       /// <returns></returns>
        public static List<Mod> GetFullModsListForGame(this MasterList m)
        {
            string gameName = Helpers.GeneralHelpers.GetGameName();

            return m.Games.FirstOrDefault(x => x.GameName == gameName)?
                .Mods;
        }

        /// <summary>
        /// Gets list of mods selected by user, with essential mods inlcuded at end of list
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static List<ModListItem> GetFullRequiredDownloadsList(this MasterList m)
        {
            // First get items selected from mods list
            var selectedToInstall = new List<ModListItem>(Static.StaticData.UserDataStore.CurrentUserData.SelectedToInstall);

            // Then append essential mods
            selectedToInstall.AddRange(m.GetEssentialModsList().Select(mod => new ModListItem
            {
                Index = 0,
                Mod = mod,
                IsChecked = true,
                IsInstalled = false,
            }).ToList());

            //Then mark direct downloads
            foreach (var mod in selectedToInstall)
            {
                mod.IsDirectDownloadOnly = mod.Mod.Files.All(f => !string.IsNullOrEmpty(f.DirectDownloadUrl));
            }

            return selectedToInstall;
        }

        public static bool HasModUpdates(this MasterList m)
        {
            return m.GetModUpdates().Count > 0;
        }

        /// <summary>
        /// Returns list of mods for which there is an update, or the mod is new
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static List<Mod> GetModUpdates(this MasterList m)
        {
            bool isGameUpdate = Static.StaticData.UserDataStore.CurrentUserData.GameVersion != Static.StaticData.GetCurrentGame().GameVersion;

            var updates = m.GetFullModsListForGame()
                .Where(mod => (ModHelpers.IsUpdate(mod) || ModHelpers.IsNewMod(mod))
                && ModHelpers.UserCanInstall(mod)
                && ((!isGameUpdate && !mod.IsEssential) || isGameUpdate)) // if game update, let users choose new non-essential mods
                .ToList();

            return updates;
        } 

        /// <summary>
        /// Gets list of mods to be reinstalled during update process. List should include all
        /// previously installed mods from user's Installed mod IDs list, but NOT those mods which
        /// are to be updated. Updated mods will have new entry instead.
        /// List should also include mods that are new to the master list.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static List<Mod> GetModListForReinstall(this MasterList m)
        {
            UserDataBase user = Static.StaticData.UserDataStore.CurrentUserData;

            List<Mod> result = new List<Mod>();

            //List of all mods, filtered by user's InstallProfile
            var availableMods = m.GetFullModsListForGame()
                .Where(ModHelpers.UserCanInstall);
            
            foreach (var mod in availableMods)
            {
                if (ModHelpers.IsUpdate(mod))
                {
                    result.Add(mod);
                }
                else if (ModHelpers.IsInstalled(mod) && result.All(r => r.Id != mod.Id)) // is installed and will not be replaced by updated version
                {
                    result.Add(mod);
                }
                else if (ModHelpers.IsNewMod(mod))
                {
                    result.Add(mod);
                }
               
            }

            return result;
        }       

        public static ListItemBase GetItemById(this MasterList m, Guid id)
        {
            foreach (GameItem game in m.Games)
            {
                if (game.Id == id)
                    return game;
                foreach (Mod mod in game.Mods)
                {
                    if (mod.Id == id)
                        return mod;
                    foreach (ModZipFile zip in mod.Files)
                    {
                        if (zip.Id == id)
                            return zip;

                        foreach (ModZipContent c in zip.Content)
                        {
                            if (c.Id == id)
                                return c;
                        }
                    }
                }
            }

            throw new Exception("No mod found!");
        }

        public static Mod GetParentMod(this MasterList m, Guid id)
        {
            foreach (GameItem game in m.Games)
            {
                foreach (Mod mod in game.Mods)
                {
                    foreach (ModZipFile zip in mod.Files)
                    {
                        if (zip.Id == id)
                            return mod;

                        foreach (ModZipContent c in zip.Content)
                        {
                            if (c.Id == id)
                                return mod;
                        }
                    }
                }
            }

            throw new Exception("No mod found!");
        }

        public static ModZipFile GetZipFileByZipName(this MasterList m, string name)
        {
            foreach (GameItem game in m.Games)
            {
                foreach (Mod mod in game.Mods)
                {
                    foreach (ModZipFile zip in mod.Files)
                    {
                        if (zip.FileName == name)
                            return zip;
                    }
                }
            }

            throw new Exception("No mod found!");
        }

        public static bool TryGetZipFileByZipName(this MasterList m, string name, out ModZipFile zipFile)
        {
            zipFile = null;

            var currentGame = m.Games.FirstOrDefault(g => g.Game == Static.StaticData.CurrentGame);

            if (currentGame == null)
                return false;

            foreach (Mod mod in currentGame.Mods)
            {
                foreach (ModZipFile zip in mod.Files)
                {
                    if (zip.FileName == name)
                    {
                        zipFile = zip;
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion Public Methods
    }
}