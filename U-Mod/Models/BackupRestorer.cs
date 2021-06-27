using U_Mod.Helpers;
using U_Mod.Static;
using System;
using System.IO;
using System.IO.Compression;
using U_Mod.Shared.Models;
using U_Mod.Shared.Enums;
using System.Linq;

namespace U_Mod.Models
{
    public class BackupRestorer
    {
        #region Public Properties

        public bool IsReinstall { get; set; }

        #endregion Public Properties

        #region Public Events

        public event EventHandler RestoreComplete;

        #endregion Public Events

        #region Public Methods

        public bool RestoreBackupForCurrentGame()
        {
            string backupZip = Constants.UModBackup;

            string zipPath = Path.Combine(FileHelpers.GetGameFolder(), Static.Constants.UMod, backupZip);

            if (!File.Exists(zipPath))
            {
                throw new Exception($"Backup file not found: {zipPath}");
            }

            if (string.IsNullOrEmpty(FileHelpers.GetGameFolder()) || !Directory.Exists(FileHelpers.GetGameFolder()))
            {
                if (!CheckReinstallComplete())
                    return false;

                OnRestoreComplete(null);
            }
            else
            {
                DirectoryInfo gameFolder = new DirectoryInfo(FileHelpers.GetGameFolder());

                //Extract to temp directory first.
                string extractDirName = Path.Combine(FileHelpers.GetGameFolder(), Static.Constants.UMod, "temp-extract");
                if(!Directory.Exists(extractDirName))
                    ZipFile.ExtractToDirectory(zipPath, extractDirName);
                
                //Then delete all files in game directory
                foreach (var d in gameFolder.EnumerateDirectories())
                {
                    if (d.FullName.Contains(Static.Constants.UMod))
                        continue;

                    if (d.FullName.Contains("Temp"))
                        continue;

                    d.Delete(true);
                }

                foreach (var f in gameFolder.EnumerateFiles())
                {
                    if (f.FullName.EndsWith(backupZip)) // don't zip itself!
                        continue;

                    f.Delete();
                }

                //Then shift from temp folder to game folder
                FileHelpers.MoveDirectory(extractDirName, FileHelpers.GetGameFolder());
                
                //DirectoryInfo extractDir = new DirectoryInfo(extractDirName);
                //foreach (var d in extractDir.EnumerateDirectories())
                //{
                //    FileHelpers.MoveDirectory(d.FullName, Path.Combine(FileHelpers.GetGameFolder(), d.Name));
                //}

                //foreach (var f in extractDir.EnumerateFiles())
                //{
                //    f.MoveTo(Path.Combine(FileHelpers.GetGameFolder(), f.Name), true);
                //}

                if (!CheckReinstallComplete())
                    return false;

                //Delete temp-extract directory
                DirectoryInfo tempExtract = new DirectoryInfo(extractDirName);
                tempExtract.Delete(true);

                OnRestoreComplete(null);
            }

            return true;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnRestoreComplete(EventArgs e)
        {
            if (this.IsReinstall)
            {
                StaticData.UserDataStore.CurrentUserData.InstalledModIds = new System.Collections.Generic.List<ModVersionInfo>();
                StaticData.UserDataStore.CurrentUserData.ByPassedModIds = new System.Collections.Generic.List<ModVersionInfo>();
                StaticData.UserDataStore.CurrentUserData.InstallationComplete = false;

                StaticData.SaveAppData(this.IsReinstall);
                
                switch (Static.StaticData.CurrentGame)
                {
                    case GamesEnum.Oblivion:
                        StaticData.AppData.OblivionGameFolder = "";
                        break;

                    case GamesEnum.Fallout:
                        StaticData.AppData.FalloutGameFolder = "";
                        break;
                }

                StaticData.SaveAppData(this.IsReinstall);
            }

            EventHandler handler = RestoreComplete;
            handler?.Invoke(this, e);
        }

        #endregion Protected Methods

        private bool CheckReinstallComplete()
        {
            DirectoryInfo d = new DirectoryInfo(FileHelpers.GetGameFolder());

            switch (Static.StaticData.CurrentGame)
            {
                case GamesEnum.Oblivion:
                case GamesEnum.Fallout:
                    //4gb patch creates "...exe.backup" files so check that those are gone
                    return !d.GetFiles().Any(f => f.FullName.ToLower().Contains("exe.backup"));

                default:
                    throw new Exception("BackupRestorer.cs - CheckReinstallComplete(). There is no handler for this game!");
            }
           
        }
    }
}