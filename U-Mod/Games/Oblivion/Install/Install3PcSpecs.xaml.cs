using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Models;

namespace U_Mod.Games.Oblivion.Pages.Install
{
    /// <summary>
    /// Interaction logic for Install3.xaml
    /// </summary>
    public partial class Install3PcSpecs : UserControl
    {
        public SystemInfo SystemInfo { get; set; }
        public Install3PcSpecs()
        {
            this.DataContext = this;
            this.SystemInfo = Static.StaticData.SystemInfo;
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.OblivionInstall2SelectGameFolder);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.OblivionMainMenu);
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            YesButton.Background = new SolidColorBrush(Colors.LightBlue);
            NoButton.Background = new SolidColorBrush(Colors.LightGray);
            Static.StaticData.UserDataStore.OblivionUserData.CanFullInstall = true;
            Static.StaticData.SaveAppData();
            Navigation.NavigateToPage(PagesEnum.OblivionInstall4ModsList, true);
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            NoButton.Background = new SolidColorBrush(Colors.LightBlue);
            YesButton.Background = new SolidColorBrush(Colors.LightGray);
            Static.StaticData.UserDataStore.OblivionUserData.CanFullInstall = false;
            Static.StaticData.SaveAppData();
            Navigation.NavigateToPage(PagesEnum.OblivionInstall4ModsList, true);
        }
    }
}
