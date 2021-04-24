using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using U_Mod.Enums;
using U_Mod.Helpers;

namespace U_Mod.Games.Oblivion.Pages.Install
{
    /// <summary>
    /// Interaction logic for Install1.xaml
    /// </summary>
    public partial class Install1PreInstallVid : UserControl
    {
        #region Public Constructors

        public Install1PreInstallVid()
        {
            InitializeComponent();

        }


        #endregion Public Constructors

        #region Private Methods

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.OblivionInstall2SelectGameFolder);
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.OblivionMainMenu, true);
        }

        #endregion Private Methods

    }
}