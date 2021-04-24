using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Models;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace U_Mod.Games.Oblivion.Pages
{
    /// <summary>
    /// Interaction logic for OblivionOptions.xaml
    /// </summary>
    public partial class OblivionOptions : UserControl
    {
        #region Public Constructors

        public OblivionOptions()
        {
            InitializeComponent();

            if (!Static.StaticData.UserDataStore.CurrentUserData.InstallationComplete)
            {
                ReinstallOblivionButton.IsEnabled = false;
                ReinstallOblivionButton.Opacity = 0.6;
            }
        }

        #endregion Public Constructors

        #region Private Methods

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.OblivionMainMenu, true);
        }

        private void LaunchObmmButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.LaunchModManager();
        }

        private void ReinstallOblivionButton_Click(object sender, RoutedEventArgs e)
        {
            Custom.CustomYesNoMessage yesNoMessage = new Custom.CustomYesNoMessage("This will restore your game to vanilla so you can reinstall the mod pack. Are you sure?");
            yesNoMessage.YesClicked += (s, e) =>
            {
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            ButtonsArea.Visibility = Visibility.Collapsed;
                            LoadingMessage.Visibility = Visibility.Visible;
                            BackButton.IsEnabled = false;
                            BackButton.Opacity = 0.7;
                        }));

                        BackupRestorer backupRestorer = new BackupRestorer();
                        backupRestorer.IsReinstall = true;
                        _ = backupRestorer.RestoreBackupForCurrentGame();

                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            GeneralHelpers.ShowMessageBox($"Game restored.\n\nYou can now perform a clean reinstall.");
                            Navigation.NavigateToPage(Enums.PagesEnum.OblivionInstall2SelectGameFolder, true);
                        }));
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Logging.Logger.LogException("RestoreOriginalGameData", ex);
                            GeneralHelpers.ShowMessageBox($"Failed to restore game backup! Cannot continue.\n\nError:{ex.Message}");
                            ButtonsArea.Visibility = Visibility.Visible;
                            LoadingMessage.Visibility = Visibility.Collapsed;
                            BackButton.IsEnabled = true;
                            BackButton.Opacity = 1;
                        }));
                    }
                });
                thread.Start();

            };
            yesNoMessage.ShowDialog();
        }

        //private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        //{
        //    ScrollViewer scv = (ScrollViewer)sender;
        //    scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
        //    e.Handled = true;
        //}

        //private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    TopCirlces.Visibility = e.VerticalOffset < 10 ? Visibility.Hidden : Visibility.Visible;
        //    BottomCircles.Visibility = e.ExtentHeight - e.VerticalOffset < 400 ? Visibility.Hidden : Visibility.Visible;

        //    e.Handled = true;
        //}

        #endregion Private Methods
    }
}