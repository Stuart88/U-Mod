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
    /// Interaction logic for Install7_4GBRamPatch.xaml
    /// </summary>
    public partial class Install7_4GBRamPatch : UserControl
    {
        private bool StopChecking { get; set; }
        public Install7_4GBRamPatch()
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
                CheckPatchInstalled();
                if (this.StopChecking)
                    timer.Stop();
            };
            timer.Start();
        }

        private void CheckPatchInstalled()
        {
            if (File.Exists(Path.Combine(FileHelpers.GetGameFolder(), "Oblivion.exe.Backup"))
            && File.Exists(Path.Combine(FileHelpers.GetGameFolder(), "OblivionLauncher.exe.Backup")))
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
            Navigation.NavigateToPage(PagesEnum.OblivionInstall8ModManager);
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.Run4GbPatch();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.OblivionMainMenu, true);
        }

        void UpdateUserData(bool val)
        {
            Static.StaticData.UserDataStore.OblivionUserData.On4GbRamPatch = val;
            Static.StaticData.SaveAppData();
        }
    }
}
