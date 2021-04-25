using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using U_Mod.Enums;
using U_Mod.Static;
using U_Mod.Helpers;
using U_Mod.Pages.BaseClasses;
using AMGWebsite.Shared.Models;
using System.Diagnostics;

namespace U_Mod.Pages.InstallBethesda
{
    public partial class _3ManualDownload : ModDownloaderBase, INotifyPropertyChanged
    {
        #region Private Fields

        private readonly object _lockObj = new object();
        private ObservableCollection<ModListItem> _listData = new ObservableCollection<ModListItem>();

        #endregion Private Fields

        #region Public Properties

        public ObservableCollection<ModListItem> ListData
        {
            get { return _listData; }
            set
            {
                _listData = value;
                OnPropertyChanged();
            }
        }

        #endregion Public Properties

        #region Private Properties

        private List<string> DownloadUrls { get; set; } = new List<string>();

        /// <summary>
        /// List of exact file paths to check for when download is complete and files have.
        /// When file is detected, list will show check mark
        /// </summary>
        //private List<(string path, ModListItem mod, string url, ModZipFile zip)> FilesToCheckFor { get; set; } = new List<(string path, ModListItem mod, string url, ModZipFile zip)>();

        private bool StopDownloadsCheck { get; set; }

        /// <summary>
        /// List of Urls that should not be opened because these files already exist in AMG folder
        /// </summary>
        private List<string> UrlsDone { get; set; } = new List<string>();

        #endregion Private Properties

        #region Public Constructors

        public _3ManualDownload()
        {
            this.DataContext = this;

            ResetItemIndices();
            SortDownloadUrls();
            this.ModsToDownload = this.ModsToDownload.GroupBy(m => m.Mod.Id).Select(g => g.First()).ToList(); // Ensure distinct here. There can be some overlap after selecting mods for update
            this.ListData = new ObservableCollection<ModListItem>(this.ModsToDownload.OrderBy(i => i.Index));

            InitializeComponent();

            CheckForDownloads();

            this.GameName.Text = GeneralHelpers.GetGameName();

            this.StopDownloadsCheck = false;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) =>
            {
                CheckForDownloads();
                if (this.StopDownloadsCheck)
                    timer.Stop();
            };
            timer.Start();

        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion Public Events

        #region Public Methods

        public override void BeginInstallProcess()
        {
            Application.Current.MainWindow.IsEnabled = false;

            this.StopDownloadsCheck = true;

            ScrollerGrid.Visibility = Visibility.Collapsed;
            InstallerGrid.Visibility = Visibility.Visible;

            CreateDirectDownloadsList();

            this.FileToAutomaticallyDownload = this.DirectDownloadsList.Count;
            this.FilesToProcess = this.DownloadUrls.Count + this.DirectDownloadsList.Count;

            if (this.DirectDownloadsList.Count > 0)
            {
                UpdateProgressText("Downloading extra files...");
                UpdateProgressBar(0);
                //Starts with first download, then when download is done, it is popped from list and if there are more urls, it moves onto the next one
                // (See end of DownloadFileCompleted)
                DirectDownload(this.DirectDownloadsList[0]);
            }
            else
            {
                ProcessFiles();
            }
        }

        public override void UpdateSecondaryProgressBar(string text, int value, int max)
        {
            SecondaryProgressFrame.Visibility = Visibility.Visible;
            SecondaryProgressGrid.Visibility = Visibility.Visible;
            SecondaryProcessingProgressBar.Maximum = max;
            SecondaryProcessingProgressBar.Value = value;
            SecondaryInstallingInfo.Text = text;
        }

        public override void UpdateProgressBar(double value)
        {
            ProcessingProgressBar.Value = value;
        }

        public override void UpdateProgressText(string value)
        {
            LoadingSpinner.Visibility = Visibility.Hidden;

            if (value.StartsWith("DOWNLOADING"))
                value = value.Replace("DOWNLOADING", ""); //Remove as it's just marker text, used in Nexus page...
            else if (value.StartsWith("Preparing") || value.StartsWith("Creating"))
            {
                SecondaryProgressFrame.Visibility = Visibility.Hidden;
                SecondaryProgressGrid.Visibility = Visibility.Collapsed;
                LoadingSpinner.Visibility = Visibility.Visible;
            }
            else
            {
                SecondaryProgressFrame.Visibility = Visibility.Visible;
                SecondaryProgressGrid.Visibility = Visibility.Visible;
                LoadingSpinner.Visibility = Visibility.Collapsed;
            }

            InstallingInfo.Text = value;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Protected Methods

        #region Private Methods

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.ModsSelect);
        }

        /// <summary>
        /// Checks for existence of required files in AMG folder, and marks as 'IsDownloaded' if the files are there
        /// </summary>
        private void CheckForDownloads()
        {
            lock (_lockObj)
            {
                foreach (var m in this.ListData)
                {
                    foreach (var f in m.Mod.Files)
                    {
                        string filePath = Path.Combine(FileHelpers.GetGameFolder(), Static.Constants.UMod, f.FileName);
                        if (File.Exists(filePath))
                        {
                            if (!this.UrlsDone.Contains(f.ManualDownloadUrl))
                                this.UrlsDone.Add(f.ManualDownloadUrl);

                            if (!this.FinalFilePaths.Contains((filePath, f)))
                                this.FinalFilePaths.Add((filePath, f));
                        }
                        else
                        {
                            this.UrlsDone.Remove(f.ManualDownloadUrl);
                            this.FinalFilePaths.Remove((filePath, f));
                        }

                        bool allDone = m.Mod.Files
                            .Where(file => !string.IsNullOrEmpty(file.ManualDownloadUrl))
                            .All(file => File.Exists(Path.Combine(FileHelpers.GetGameFolder(), Static.Constants.UMod, file.FileName)));

                        m.IsDownloaded = allDone;

                        OnPropertyChanged(nameof(this.ListData));
                        OnPropertyChanged(nameof(m));
                    }

                }

                List<ModZipFile> allFiles = this.ListData.SelectMany(d => d.Mod.Files).Where(f => !string.IsNullOrEmpty(f.ManualDownloadUrl)).ToList();
                if (allFiles.All(f => File.Exists(Path.Combine(FileHelpers.GetGameFolder(), Static.Constants.UMod, f.FileName))))
                {
                    InstallButton.Opacity = 1;
                    InstallButton.IsEnabled = true;
                }
                else
                {
                    InstallButton.Opacity = 0.6;
                    InstallButton.IsEnabled = false;
                }

            }
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (((Grid)sender).Tag is int index)
            {
                ModListItem updating = this.ListData.First(x => x.Index == index);

                List<string> urls = updating.Mod.Files
                    .Where(f => !string.IsNullOrEmpty(f.ManualDownloadUrl))
                    .Select(s => s.ManualDownloadUrl)
                    .Where(url => !this.UrlsDone.Contains(url))
                    .ToList();

                foreach (string u in urls)
                {
                    ProcessHelpers.OpenInBrowser(u);
                }
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            BeginInstallProcess();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(GeneralHelpers.GetMainMenuPageEnumForGame());
        }


        /// <summary>
        /// Ensures the set of download Urls is distinct, i.e. mods don't link to a url if another one already has that url.
        /// This also ensures only Manual Download Urls are opened in browser. Direct downloads are handled automatically in BeginInstallProcess()
        /// </summary>
        private void SortDownloadUrls()
        {
            foreach (var m in this.ModsToDownload)
            {
                foreach (var f in m.Mod.Files)
                {
                    if (!this.DownloadUrls.Contains(f.ManualDownloadUrl))
                    {
                        this.DownloadUrls.Add(f.ManualDownloadUrl);
                    }
                }
            }
        }

        public void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #endregion Private Methods

        private void UModFolderLink_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("explorer.exe", FileHelpers.GetUModFolder());
        }
    }
}