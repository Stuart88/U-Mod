using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Models;

namespace U_Mod.Games.Fallout.Pages.InstallFallout
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
            Navigation.NavigateToPage(PagesEnum.FalloutInstall2SelectGameFolder);
        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutInstall4ModsList, true);
        }
        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutMainMenu);
        }


        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            Static.StaticData.UserDataStore.CurrentUserData.CanFullInstall = true;
            Static.StaticData.SaveAppData();
            YesButton.Background = new SolidColorBrush(Colors.LightBlue);
            NoButton.Background = new SolidColorBrush(Colors.LightGray);
            NoFrame.BorderBrush = new SolidColorBrush(Colors.Transparent);
            EnableNextButton();
        }

        private void EnableNextButton()
        {
            NextButton.Opacity = 1;
            NextButton.IsEnabled = true;
        }

        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            Static.StaticData.UserDataStore.FalloutUserData.CanFullInstall = false;
            Static.StaticData.SaveAppData();
            NoButton.Background = new SolidColorBrush(Colors.LightBlue);
            YesButton.Background = new SolidColorBrush(Colors.LightGray);
            YesFrame.BorderBrush = new SolidColorBrush(Colors.Transparent);
            EnableNextButton();
        }
    }
}
