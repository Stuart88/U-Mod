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
            LaunchButton.IsEnabled = true;
            LaunchButton.Opacity = 1;
#else
            NextButton.IsEnabled = false;
            NextButton.Opacity = 0.7;
            LaunchButton.IsEnabled = false;
            LaunchButton.Opacity = 0.7;
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
            string fileName = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => "OblivionLauncher.exe.Backup",
                GamesEnum.Fallout => "Fallout3.exe.Backup"
            };
            if (File.Exists(Path.Combine(FileHelpers.GetGameFolder(), fileName)))
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
            Navigation.NavigateToPage(PagesEnum.FalloutInstall8ModManager);
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.Run4GbPatch();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutMainMenu, true);
        }

        void UpdateUserData(bool val)
        {
            Static.StaticData.UserDataStore.FalloutUserData.On4GbRamPatch = val;
            Static.StaticData.SaveAppData();
        }
    }
}
