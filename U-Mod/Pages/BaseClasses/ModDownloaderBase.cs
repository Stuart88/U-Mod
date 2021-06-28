using U_Mod.Enums;
using U_Mod.Extensions;
using U_Mod.Helpers;
using U_Mod.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Application = System.Windows.Application;
using UserControl = System.Windows.Controls.UserControl;
using U_Mod.Static;
using U_Mod.Helpers.GameSpecific;
using U_Mod.Shared.Models;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Helpers;
using U_Mod.Pages.UserControls;

namespace U_Mod.Pages.BaseClasses
{
    public abstract partial class ModDownloaderBase : UserControl
    {
        #region Private Fields

        private readonly object _downloadLock = new object();

        #endregion Private Fields

        #region Public Properties

        public string ApiKey { get; set; }
        public List<DirectDownloadData> DirectDownloadsList { get; set; } = new List<DirectDownloadData>();
        public List<FileExtractResult> ExtractionResults { get; set; } = new List<FileExtractResult>();
        public int FilesToProcess { get; set; }
        public int FileToAutomaticallyDownload { get; set; }
        public LoadingMessages LoadingMessages { get; set; } = new LoadingMessages() 
        { 
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center, 
            Visibility = System.Windows.Visibility.Collapsed 
        };

        /// <summary>
        /// The final points where all necessary zip files exist, used in the file extraction stage.
        /// </summary>
        public List<(string path, ModZipFile zip)> FinalFilePaths { get; set; } = new List<(string path, ModZipFile zip)>();

        public List<ModListItem> ModsToDownload { get; set; }
        public string NexusConnectionToken { get; set; }
        public int ProcessingCount { get; set; }

        #endregion Public Properties

        #region Private Properties

        private int DownloadAttempts { get; set; } = 0;
        private int DownloadedCount { get; set; } = 0;
        private PagesEnum MenuPage { get; set; }
        private PagesEnum NextPage { get; set; }
        private PagesEnum PreviousPage { get; set; }

        #endregion Private Properties

        #region Protected Constructors

        protected ModDownloaderBase()
        {
           
            MenuPage = PagesEnum.MainMenu;
            NextPage = PagesEnum.PatchAndModManager;
            PreviousPage = PagesEnum.ManualDownload;

            if (GeneralHelpers.GetUserDataForGame().IsUpdating)
            {
                this.ModsToDownload = Static.StaticData.UserDataStore.CurrentUserData.SelectedToInstall;
            }
            else
            {
                this.ModsToDownload = Static.StaticData.MasterList.GetFullRequiredDownloadsList();
            }
        }

        #endregion Protected Constructors

        #region Public Methods

        public abstract void BeginInstallProcess();

        public void CreateDirectDownloadsList(bool includeAutoDownload = false)
        {
            this.DirectDownloadsList = this.ModsToDownload
                // Find only mod files that have a DirectDownload Url
                .Where(d => !ModHelpers.IsInstalled(d.Mod))
                .SelectMany(m => m.Mod.Files)
                .Where(f => !string.IsNullOrEmpty(f.DirectDownloadUrl))
                .Select(s => new DirectDownloadData
                {
                    DirectDownloadUrl = s.DirectDownloadUrl,
                    FileName = s.FileName,
                    DownloadedPath = Path.Combine(Helpers.FileHelpers.GetUModFolder(), s.FileName),
                    ZipFile = s
                })
                .ToList();

            if (includeAutoDownload)
            {
                // Append all other mod files and assign their AutoDownload Url
                this.DirectDownloadsList.AddRange(this.ModsToDownload
                    .Where(d => !ModHelpers.IsInstalled(d.Mod))
                    .SelectMany(m => m.Mod.Files)
                    .Where(f => string.IsNullOrEmpty(f.DirectDownloadUrl))
                    .Select(s => new DirectDownloadData
                    {
                        DirectDownloadUrl = s.AutoDownloadUrl,
                        FileName = s.FileName,
                        DownloadedPath = Path.Combine(Helpers.FileHelpers.GetUModFolder(), s.FileName),
                        ZipFile = s,
                        IsAutoDownload = true,
                    })
                    .ToList());
            }
        }

        public void DirectDownload(DirectDownloadData downloadData)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (this.DownloadAttempts == 0) // don't need to update anything on retries
                {
                    string modName = Static.StaticData.MasterList.GetParentMod(downloadData.ZipFile.Id).ModName;

                    UpdateProgressText($"DOWNLOADING{modName.TrimToLength(30)}: {this.DownloadedCount++}/{this.FileToAutomaticallyDownload}");
                    // Note, the "DOWNLOADING" part is a marker to tell the Nexus page what to do. See UpdateProgressText() on Nexus page

                    double progress = this.FileToAutomaticallyDownload == 0 ? 100 : (((double)this.DownloadedCount / this.FileToAutomaticallyDownload) * 100d);
                    UpdateProgressBar(progress);
                }
            });

            if (ZipHelpers.VerifiyZip(downloadData.DownloadedPath, downloadData.ZipFile, out _))
            {
                OnDownloadFileCompleted(downloadData.DownloadedPath, downloadData.ZipFile, true);
            }
            else
            {
                try
                {
                    Thread thread = new Thread(async () =>
                    {
                        bool downloadOk = true;

                        MyWebClient client = new MyWebClient(40000);
                        client.DownloadFileCompleted += (s, e) =>
                        {
                            //timer.Stop();
                            //timer.Enabled = false;
                            bool zipVerified = ZipHelpers.VerifiyZip(downloadData.DownloadedPath, downloadData.ZipFile, out string resultMsg);

                            if (e.Cancelled || e.Error is WebException)
                            {
                                DownloadNotOkay("Download terminated due to poor internet connection.");
                            }
                            else if (zipVerified)
                            {
                                OnDownloadFileCompleted(downloadData.DownloadedPath, downloadData.ZipFile, downloadOk);
                            }
                            else
                            {
                                // If zip file was corrupt, delete it here
                                if (File.Exists(downloadData.DownloadedPath))
                                    File.Delete(downloadData.DownloadedPath);

                                DownloadNotOkay(resultMsg);
                            }
                        };
                        client.DownloadProgressChanged += (s, e) =>
                        {
                            //timer.Start();
                            Application.Current.Dispatcher.Invoke(() => { UpdateSecondaryProgressBar($"{downloadData.FileName.TrimToLength(20)}", e.ProgressPercentage, 100); });
                        };

                        if (downloadData.IsAutoDownload)
                        {
                            try
                            {
                                var resp = await HttpHelpers.FetchFromNexus<List<NexusGeneratedDownloadLinkResponse>>(downloadData.ZipFile.AutoDownloadUrl, null, this.ApiKey);

                                if (resp.Ok)
                                {
                                    if (resp.Data.Count > 0)
                                    {
                                        client.Headers.Add("apikey", this.ApiKey);
                                        client.DownloadFileAsync(new Uri(resp.Data[0].Uri), downloadData.DownloadedPath);
                                    }
                                    else
                                    {
                                        DownloadNotOkay();
                                    }
                                }
                                else
                                {
                                    DownloadNotOkay(resp.ErrorMessage);
                                }
                            }
                            catch (Exception e)
                            {
                                Logging.Logger.LogException("DirectDownload (1)", e);
                                DownloadNotOkay(e.Message);
                            }
                        }
                        else
                        {
                            client.DownloadFileAsync(new Uri(downloadData.DirectDownloadUrl), downloadData.DownloadedPath);
                        }

                        void DownloadNotOkay(string message = "")
                        {
                            //client.Dispose();

                            if (this.DownloadAttempts < 3)
                            {
                                //Try again
                                this.DownloadAttempts++;
                                DirectDownload(DirectDownloadsList[0]);
                            }
                            else
                            {
                                downloadOk = false;
                                string manualDownloadUrl = string.IsNullOrEmpty(downloadData.ZipFile.ManualDownloadUrl) ? downloadData.ZipFile.DirectDownloadUrl : downloadData.ZipFile.ManualDownloadUrl;
                                this.ExtractionResults.Add(new FileExtractResult
                                {
                                    DownloadOk = false,
                                    FilePath = downloadData.DownloadedPath,
                                    Message = $"Failed to download file:\n {downloadData.FileName}\nMessage: {message}\nManual Download URL: {manualDownloadUrl}",
                                    Result = ZipFileWorker.FileExtractResultEnum.Fail
                                });

                                OnDownloadFileCompleted(downloadData.DownloadedPath, downloadData.ZipFile, downloadOk);
                            }
                        }
                    });
                    thread.Start();
                }
                catch (Exception e)
                {
                    // Navigates back to menu on exception
                    GeneralHelpers.ShowExceptionMessageBox("Fatal Error!", e);
                    Logging.Logger.LogException("DirectDownload (2)", e);
                    Navigation.NavigateToPage(MenuPage, true);
                }
            }
        }

        /// <summary>
        /// Performs any final game-specific steps then navigates to the game-specific next page
        /// </summary>
        public void NavigateToNextPage()
        {
            Application.Current.MainWindow.IsEnabled = true;
            Navigation.NavigateToPage(NextPage);
        }

        public bool PostInstallFileEdits()
        {
            switch (Static.StaticData.CurrentGame)
            {
                case GamesEnum.Oblivion:
                    OblivionTools oblivionTool = new OblivionTools();

                    string stage = "";
                    string currentFile = "";
                    try
                    {
                        stage = "(0)";
                        oblivionTool.PerformFileEdits(out currentFile);
                        stage = "(1)";
                        oblivionTool.EnableAntiPiracyMeasures(out currentFile);
                        stage = "(2)";
                        oblivionTool.ObseIniFileEnableAntiPiracyEdit(out currentFile);
                        return true;
                    }
                    catch (Exception e)
                    {
                        GeneralHelpers.ShowMessageBox($"Post-install file edits {stage} failed at {currentFile}\n\nError: {Shared.Helpers.StringHelpers.ErrorMessage(e)}");
                        Logging.Logger.LogException("PostInstallFileEdits", e);
                        Navigation.NavigateToPage(Enums.PagesEnum.MainMenu, true);
                        return false;
                    }

                default:
                    return true;
            }
        }

        public void ProcessFiles()
        {
            UserDataBase user = GeneralHelpers.GetUserDataForGame();

            Thread thread = new Thread(() =>
            {
                int debugtrack = 0;
                try
                {
                    this.ProcessingCount = 1;

                    if (Static.StaticData.UserDataStore.CurrentUserData.IsUpdating)
                    {
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            debugtrack = 7;
                            UpdateProgressText("Preparing for update...");
                        });
                        debugtrack = 1;
                        RestoreOriginalGameData();
                        debugtrack = 2;
                        this.FinalFilePaths = Static.StaticData.MasterList.GetModListForReinstall()
                            .SelectMany(m => m.Files)
                            .Select(zip => (Path.Combine(Helpers.FileHelpers.GetUModFolder(), zip.FileName), zip))
                            .OrderBy(i => Static.StaticData.MasterList.GetParentMod(i.zip.Id).InstallOrder)
                            .ThenBy(i => i.zip.InstallOrder)
                            .ToList();
                        debugtrack = 3;
                    }
                    else
                    {
                        debugtrack = 4;
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            debugtrack = 5;
                            UpdateProgressText("Creating game backup. This might take a while...");
                            this.LoadingMessages.Visibility = System.Windows.Visibility.Visible;
                            this.LoadingMessages.StartCyclingMessages();
                        });
                        debugtrack = 6;

                        ZipOriginalGameData();

                        switch (Static.StaticData.CurrentGame)
                        {
                            // Doing this as a switch as it's cleaner for handling other cases that might be added
                            case GamesEnum.Fallout:
                                debugtrack = 17;
                                ZipFalloutIniFiles();
                                break;
                        }

                        debugtrack = 8;
                        this.FinalFilePaths = this.FinalFilePaths
                            .OrderBy(i => Static.StaticData.MasterList.GetParentMod(i.zip.Id).InstallOrder)
                            .ThenBy(i => i.zip.InstallOrder)
                            .ToList();
                        debugtrack = 9;
                    }

                    // file verification

                    {
                        List<(bool ok, string fileName, string error)> verificationResults = new List<(bool ok, string fileName, string error)>();
                        debugtrack = 10;
                        foreach (var p in FinalFilePaths)
                        {
                            if (U_Mod.Static.StaticData.MasterList.TryGetZipFileByZipName(Path.GetFileName(p.path), out ModZipFile zip))
                            {
                                bool res = ZipHelpers.VerifiyZip(p.path, zip, out string result);
                                verificationResults.Add((res, Path.GetFileName(p.path), result));
                            }
                            else
                            {
                                verificationResults.Add((false, Path.GetFileName(p.path), "Zip Not Found!"));
                            }
                        }
                        debugtrack = 11;
                        if (verificationResults.Any(r => !r.ok))
                        {
                            debugtrack = 12;
                            string errorStr = "The following mods files encountered problems. Please manually check for their integrity and correctness then try again.\n\n";
                            foreach (var r in verificationResults.Where(r => !r.ok))
                            {
                                errorStr += $"File: {r.fileName}\nError: {r.error}\n\n";
                            }

                            Logging.Logger.LogException("Mod Verification Errors", new Exception(errorStr));

                            Application.Current.Dispatcher.BeginInvoke(() =>
                            {
                                GeneralHelpers.ShowMessageBox(errorStr);
                                Navigation.NavigateToPage(MenuPage, true);
                            });
                            return;
                        }
                    }
                    debugtrack = 13;
                    for (int i = 0; i < this.FinalFilePaths.Count; i++)
                    {
                        string modName = Static.StaticData.MasterList.GetParentMod(this.FinalFilePaths[i].zip.Id).ModName.TrimToLength(25);
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            UpdateProgressText($"{modName} {this.ProcessingCount}/{this.FinalFilePaths.Count}");
                        });

                        //Create temp object with required data only.
                        //Seems to be a "Collection was modified;" error happening somewhere, and there's no harm
                        //in cloning this here for safety...
                        ModZipFile temp = new ModZipFile
                        {
                            Id = this.FinalFilePaths[i].zip.Id,
                            ExtractLocation = this.FinalFilePaths[i].zip.ExtractLocation,
                            Content = new List<ModZipContent>(this.FinalFilePaths[i].zip.Content),
                            FileName = this.FinalFilePaths[i].zip.FileName
                        };

                        ZipFileWorker worker = new ZipFileWorker(this.FinalFilePaths[i].path, temp);
                        worker.UnzipProgressed += Worker_UnzipProgressed; ;
                        FileExtractResult res = worker.BeginUnzip();

                        if (res.DownloadOk && res.TransferOk && res.UnzipOk)
                        {
                            Mod mod = Static.StaticData.MasterList.GetParentMod(res.ModZipFile.Id);

                            if (ModHelpers.IsUpdate(mod, out ModVersionInfo toRemove))
                                user.InstalledModIds.Remove(toRemove);

                            if (!ModHelpers.IsInstalled(mod))
                                user.InstalledModIds.Add(new ModVersionInfo(mod));

                            Static.StaticData.SaveAppData();
                        }

                        this.ExtractionResults.Add(res);
                        Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            double progress = this.FinalFilePaths.Count == 0 ? 100 : (((double)this.ProcessingCount++ / this.FinalFilePaths.Count) * 100d);
                            UpdateProgressBar(progress);
                        });
                    }
                    debugtrack = 14;
                    // Finally add list of all mods that were bypassed
                    foreach (var mod in Static.StaticData.MasterList.GetFullModsListForGame())
                    {
                        if (!user.InstalledModIds.Any(m => m.Id == mod.Id && m.Version == mod.Version))
                        {
                            if (!user.ByPassedModIds.Any(m => m.Id == mod.Id && m.Version == mod.Version))
                            {
                                user.ByPassedModIds.Add(new ModVersionInfo(mod));
                            }
                        }
                    }
                    debugtrack = 15;
                    Static.StaticData.SaveAppData();
                    debugtrack = 16;
                }
                catch (Exception e)
                {
                    Logging.Logger.LogException($"ProcessFiles (1): DebugTrack: {debugtrack}", e);
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        GeneralHelpers.ShowExceptionMessageBox("Error extracting mods!", e);
                        Navigation.NavigateToPage(MenuPage, true);
                    });
                    return;
                }

                try
                {
                    DirectoryInfo tempFolder = new DirectoryInfo(Helpers.FileHelpers.GetFileExtractionTempFolderPath());
                    tempFolder.EmptyDirectory();
                }
                catch (Exception e)
                {
                    Logging.Logger.LogException("ProcessFiles (2)", e);
                }

                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    UpdateProgressBar(100);
                    if (this.ExtractionResults.All(res => res.UnzipOk && res.TransferOk && res.DownloadOk))
                    {
                        UpdateProgressText("Complete");
                        Task.Delay(1500);

                        if (PostInstallFileEdits())
                        {
                            DeleteObsoleteFiles();
                            NavigateToNextPage();
                        }
                        else
                        {
                            Navigation.NavigateToPage(MenuPage, true);
                        }
                    }
                    else
                    {
                        string errorStr = "The following mod files failed to install:\n\n\n";
                        foreach (var res in this.ExtractionResults.Where(res => !res.UnzipOk || !res.TransferOk || !res.DownloadOk))
                        {
                            errorStr += $"[{res.ModZipFile.FileName}]\nREASON: {res.Message}\n\n\n";
                        }

                        GeneralHelpers.ShowMessageBox(errorStr);
                        Navigation.NavigateToPage(MenuPage, true);
                    }
                });
            });
            thread.Start();
        }

        public void ResetItemIndices()
        {
            this.ModsToDownload = this.ModsToDownload.OrderByDescending(m => m.Mod.IsEssential)
                .ThenBy(m => m.Mod.ModName)
                .ToList();

            int index = 0;
            foreach (var m in this.ModsToDownload)
            {
                if (m.Mod.Files.All(f => !string.IsNullOrEmpty(f.DirectDownloadUrl)))
                    m.Index = int.MaxValue; // these are collapsed in XAML view, so put to end of list...
                else
                    m.Index = ++index;
            }
        }

        public abstract void UpdateProgressBar(double value);

        public abstract void UpdateProgressText(string value);

        public abstract void UpdateSecondaryProgressBar(string text, int value, int max);

        #endregion Public Methods

        #region Private Methods

        private void DeleteObsoleteFiles()
        {
            try
            {
                // If it's update process, need to delete any old downloads that have same file name as
                // the updates. Otherwise they'll be bypassed when it comes to downloading

                DirectoryInfo amgFolder = new DirectoryInfo(Helpers.FileHelpers.GetUModFolder());

                List<string> currentModPaths = Static.StaticData.MasterList.GetFullModsListForGame()
                    .SelectMany(m => m.Files)
                    .Select(f => Path.Combine(Helpers.FileHelpers.GetUModFolder(), f.FileName))
                    .ToList();

                foreach (var f in amgFolder.GetFiles())
                {
                    if (f.Name.Contains(Static.StaticData.GetCurrentGameDataFileName()) || f.Name.Contains(Constants.UModBackup))
                        continue;

                    if (currentModPaths.Any(p => p == f.FullName))
                        continue;

                    f.Delete();
                }
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("DeleteObsoleteFiles", e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="finalPath">Where the download now exists</param>
        private void OnDownloadFileCompleted(string finalPath, ModZipFile zip, bool downloadOk)
        {
            try
            {
                this.DownloadAttempts = 0;

                if (downloadOk)
                {
                    if (!this.FinalFilePaths.Contains((finalPath, zip)))
                        this.FinalFilePaths.Add((finalPath, zip));
                }

                try
                {
                    this.DirectDownloadsList.RemoveAt(0);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // might happen due to race condition when download finishes too quickly (or if download is bypassed)
                    // so can safely bypass this. It simply means there are no more items left to download!
                    Logging.Logger.LogException("OnDownloadFileCompleted (1)", e);
                }

                if (this.DirectDownloadsList.Count > 0)
                {
                    DirectDownload(this.DirectDownloadsList[0]);
                }
                else if (this.ExtractionResults.Any(res => !res.DownloadOk))
                {
                    string errorStr = $"The following mods failed to download. Please attempt manual download instead.\n\nCopy the given URLs below into your browser and place each downloaded file in your UMod folder: {Helpers.FileHelpers.GetUModFolder()}\n\n\n";
                    foreach (var res in this.ExtractionResults.Where(res => !res.DownloadOk))
                    {
                        errorStr += $"{res.Message}\n\n\n";
                    }

                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        GeneralHelpers.ShowMessageBox(errorStr);
                        Navigation.NavigateToPage(PreviousPage, false);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke((MethodInvoker)ProcessFiles);
                }
            }
            catch (Exception e)
            {
                GeneralHelpers.ShowExceptionMessageBox("Error encountered! Running the installer again will attempt to pick up where it left off, but if the error persists you may need to request support.", e);
                Logging.Logger.LogException("OnDownloadFileCompleted (2)", e);
                Navigation.NavigateToPage(MenuPage, true);
            }
        }

        /// <summary>
        /// Delete all game data files and restore from backup zip. Required before update and re-install procuderes
        /// </summary>
        /// <returns></returns>
        private bool RestoreOriginalGameData()
        {
            try
            {
                BackupRestorer backupRestorer = new BackupRestorer();
                return backupRestorer.RestoreBackupForCurrentGame();
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Logging.Logger.LogException("RestoreOriginalGameData", e);
                    GeneralHelpers.ShowMessageBox($"Failed to restore game backup! Cannot continue.\n\nError:{e.Message}");
                    Navigation.NavigateToPage(MenuPage, true);
                });

                return false;
            }
        }

        private void Worker_UnzipProgressed(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                UnzipProgressEventArgs args = ((UnzipProgressEventArgs)e);
                UpdateSecondaryProgressBar(args.Text, args.Count, args.Total);
            });
        }

        private bool ZipFalloutIniFiles()
        {
            try
            {
                string backupZip = "Fallout3ini.zip";
                string zipPath = Path.Combine(Helpers.FileHelpers.GetGameFolder(), Static.Constants.UMod, backupZip);

                if (!File.Exists(zipPath))
                {
                    using ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);

                    string[] iniFiles = new string[]
                    {
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout3", "Fallout.ini"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout3", "FalloutPrefs.ini"),
                        Path.Combine(Helpers.FileHelpers.GetGameFolder(), "Fallout_default.ini")
                    };

                    foreach (string s in iniFiles)
                    {
                        FileInfo f = new FileInfo(s);
                        if (f.Exists)
                            zip.CreateEntryFromFile(f.FullName, f.Name);
                        else
                            throw new Exception($"File does not exist! {f.FullName}");
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Logging.Logger.LogException("ZipFalloutIniFiles", e);
                    GeneralHelpers.ShowMessageBox($"Failed to zip Fallout ini files! Cannot continue.\n\nError:{e.Message}");
                    Navigation.NavigateToPage(Enums.PagesEnum.MainMenu, true);
                });

                return false;
            }
        }

        private bool ZipOriginalGameData()
        {
            try
            {
                string backupZip = Constants.UModBackup;
                string zipPath = Path.Combine(Helpers.FileHelpers.GetGameFolder(), Static.Constants.UMod, backupZip);

                if (!File.Exists(zipPath))
                {
                    using ZipArchive zip = ZipFile.Open(zipPath, ZipArchiveMode.Create);

                    DirectoryInfo di = new DirectoryInfo(Helpers.FileHelpers.GetGameFolder());
                    foreach (var d in di.EnumerateDirectories())
                    {
                        if (d.FullName.Contains(Static.Constants.UMod))
                            continue;

                        zip.CreateEntryFromDirectory(d.FullName, d.Name);
                    }

                    foreach (var f in di.EnumerateFiles())
                    {
                        zip.CreateEntryFromFile(f.FullName, f.Name);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    Logging.Logger.LogException("ZipOriginalGameData", e);
                    GeneralHelpers.ShowMessageBox($"Failed to create game backup! Cannot continue.\n\nError:{e.Message}");
                    Navigation.NavigateToPage(MenuPage, true);
                });

                return false;
            }
            finally{
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    this.LoadingMessages.Visibility = System.Windows.Visibility.Collapsed;
                    this.LoadingMessages.StopCyclingMessages();
                });
            }
        }

        #endregion Private Methods
    }
}