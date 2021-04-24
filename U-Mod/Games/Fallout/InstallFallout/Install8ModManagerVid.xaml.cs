using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AMGWebsite.Shared.Enums;
using U_Mod.Enums;
using U_Mod.Helpers;

namespace U_Mod.Games.Fallout.Pages.InstallFallout
{
    /// <summary>
    /// Interaction logic for Install8ModManagerVid.xaml
    /// </summary>
    public partial class Install8ModManagerVid : UserControl
    {
        private bool StopChecking { get; set; }
        public Install8ModManagerVid()
        {
            UpdateUserData(true);
            InitializeComponent();

         
#if DEV
            NextButton.IsEnabled = true;
            NextButton.Opacity = 1;
            LaunchButton.IsEnabled = true;
            LaunchButton.Opacity = 1;
#else
            NextButton.IsEnabled = false;
            NextButton.Opacity = 0.7;
            //LaunchButton.IsEnabled = false;
            //LaunchButton.Opacity = 0.7;
#endif


          
            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromMilliseconds(100);      // Does not need to check if it's installed because it's a launchable exe already
            //timer.Tick += (s, e) =>
            //{
            //    CheckModManagerInstalled();
            //    if (this.StopChecking)
            //        timer.Stop();
            //};
            //timer.Start();
        }

        private void CheckModManagerInstalled()
        {
            string exeName = Path.Combine("GeMM", "fomm.exe");

            if (File.Exists(Path.Combine(FileHelpers.GetGameFolder(), exeName)))
            {
                NextButton.IsEnabled = true;
                NextButton.Opacity = 1;
                this.StopChecking = true;

                //For Fallout, need to copy fose_loader.exe to mod manager folder (mod manager seems to need to see fallout exe)
                //Tools.CreateShortcut("Shortcut for fose_loader.exe", Path.Combine(FileHelpers.GetGameFolder(), "fose_loader.exe.shortcut"), Path.Combine(FileHelpers.GetGameFolder(), "fose_loader.exe"));
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserData(false);
            Navigation.NavigateToPage(PagesEnum.FalloutInstall9PostInstallVid, true);
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.LaunchModManager();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutMainMenu, true);
        }

        void UpdateUserData(bool val)
        {
            Static.StaticData.UserDataStore.FalloutUserData.OnModManagerPage = val;
            Static.StaticData.SaveAppData();
        }
    }
}
