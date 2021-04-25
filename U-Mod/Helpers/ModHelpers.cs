using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U_Mod.Models;
using AmgShared.Models;
using U_Mod.Extensions;
using AMGWebsite.Shared.Models;
using AMGWebsite.Shared.Enums;

namespace U_Mod.Helpers
{
    public static class ModHelpers
    {
        private static UserDataBase User => GeneralHelpers.GetUserDataForGame();
        public static bool IsInstalled(Mod m)
        {
            return User.InstalledModIds.Any(mod => mod.Id == m.Id && mod.Version == m.Version);
        }

        public static bool HasVersionInstalled(Mod m)
        {
            return User.InstalledModIds.Any(mod => mod.Id == m.Id);
        }

        public static bool HasPreviousVersion(ModZipFile modZipFile, out List<string> zipFileLocations)
        {
            zipFileLocations = new List<string>();

            var mod = Static.StaticData.MasterList.GetParentMod(modZipFile.Id);

            var previousVersion = User.InstalledModIds.FirstOrDefault(m => m.Id == mod.Id && m.Version < mod.Version);
            bool hasPreviousVersion = previousVersion != null;

            if (hasPreviousVersion)
            {
                var previousMod = Static.StaticData.MasterList.GetItemById(previousVersion.Id) as Mod;
                zipFileLocations = previousMod?.Files.Select(f => Path.Combine(FileHelpers.GetUModFolder(), f.FileName)).ToList();
            }

            return hasPreviousVersion;
        }

        /// <summary>
        /// Checks if older version of mod exists in user's Installed IDs list, and provides the item that can now be removed since
        /// it is obsolete
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public static bool IsUpdate(Mod mod, out ModVersionInfo toRemove)
        {
            

            //This will just be null if there's nothing to update
            toRemove = User.InstalledModIds.FirstOrDefault(m => m.Id == mod.Id && m.Version != mod.Version);
            
            return User.InstalledModIds.Any(m => m.Id == mod.Id && m.Version != mod.Version);
        }

        /// <summary>
        /// Returns true if mod exists in user's installed mod IDs list but has different version
        /// </summary>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static bool IsUpdate(Mod mod)
        {
            var user = GeneralHelpers.GetUserDataForGame();
            return user.InstalledModIds.Any(intalledMod => intalledMod.Id == mod.Id && intalledMod.Version != mod.Version);
        }

        public static bool UserCanInstall(Mod m)
        {
            UserDataBase u = GeneralHelpers.GetUserDataForGame();

            return (m.IsFullInstallOnly && u.CanFullInstall || !m.IsFullInstallOnly)
            && ((m.InstallProfile & u.InstallProfile) == u.InstallProfile
                || (m.InstallProfile & InstallProfileEnum.NoData) == InstallProfileEnum.NoData);
        }

        /// <summary>
        /// Returns true if mod does not exist in any of user's installed mod IDs list or user's bypassed mod IDs list
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsNewMod(Mod mod)
        {
            

            return User.ByPassedModIds.All(bypassedMod => bypassedMod.Id != mod.Id)
                   && User.InstalledModIds.All(installedMod => installedMod.Id != mod.Id);
        }
    }
}
