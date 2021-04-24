using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using U_Mod.Enums;
using U_Mod.Helpers;

namespace U_Mod.Games.Fallout.Pages.InstallFallout
{
    /// <summary>
    /// Interaction logic for Install9PostInstallVid.xaml
    /// </summary>
    public partial class Install9PostInstallVid : UserControl
    {

        public Install9PostInstallVid()
        {
            UpdateUserData(true);

            InitializeComponent();
           

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutInstall8ModManager, true);
        }

        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            EndInstaller();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            EndInstaller();
        }

        private void EndInstaller()
        {
            
            Static.StaticData.UserDataStore.CurrentUserData.GameVersion = Static.StaticData.GetCurrentGame().GameVersion;
            Static.StaticData.UserDataStore.CurrentUserData.InstallationComplete = true;
            Static.StaticData.UserDataStore.CurrentUserData.IsUpdating = false; // this might have been false anyway, but reset now just in case user was updating.
            UpdateUserData(false);
            Navigation.NavigateToPage(PagesEnum.FalloutMainMenu, true);
        }

        void UpdateUserData(bool val)
        {
            Static.StaticData.UserDataStore.FalloutUserData.OnFinalPage = val;
            Static.StaticData.SaveAppData();
        }
    }
}
