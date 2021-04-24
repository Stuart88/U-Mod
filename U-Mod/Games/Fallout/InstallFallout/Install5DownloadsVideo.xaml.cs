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
    /// Interaction logic for Install5DownloadsVideo.xaml
    /// </summary>
    public partial class Install5DownloadsVideo : UserControl
    {

        public Install5DownloadsVideo()
        {
            InitializeComponent();

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutInstall4ModsList, false);
        }

        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.FalloutInstall6DownloadsList, true);
        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.NexusLogin, true);
        }


    }
}
