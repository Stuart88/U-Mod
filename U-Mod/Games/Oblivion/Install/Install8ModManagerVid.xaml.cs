using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using U_Mod.Enums;
using U_Mod.Helpers;

namespace U_Mod.Games.Oblivion.Pages.Install
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
#endif
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += (s, e) =>
            {
                CheckModManagerInstalled();
                if (this.StopChecking)
                    timer.Stop();
            };
            timer.Start();
        }

        private void CheckModManagerInstalled()
        {
            if (File.Exists(System.IO.Path.Combine(FileHelpers.GetGameFolder(), "OblivionModManager.exe")))
            {
                NextButton.IsEnabled = true;
                NextButton.Opacity = 1;
                LaunchButton.IsEnabled = false;
                LaunchButton.Opacity = 0.6;
                this.StopChecking = true;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateUserData(false);
            Navigation.NavigateToPage(PagesEnum.OblivionInstall9PostInstallVid, true);
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.RunModManagerSetup();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.OblivionMainMenu, true);
        }

        void UpdateUserData(bool val)
        {
            Static.StaticData.UserDataStore.OblivionUserData.OnModManagerPage = val;
            Static.StaticData.SaveAppData();
        }
    }
}
