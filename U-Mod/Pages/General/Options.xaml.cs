using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Models;
using U_Mod.Static;

namespace U_Mod.Pages.General
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : UserControl
    {


        public Options()
        {
            InitializeComponent();

            ReinstallBtn.Content = $"Reinstall {GeneralHelpers.GetGameName()}";
            ReinstallBtn.IsEnabled = Static.StaticData.UserDataStore.CurrentUserData?.InstallationComplete ?? false;

            ReinstallBtn.Visibility = Static.StaticData.CurrentGame == Shared.Enums.GamesEnum.None
                ? Visibility.Collapsed
                : Visibility.Visible;

            //ErrorLogsBtn.Visibility = Static.StaticData.CurrentGame == Shared.Enums.GamesEnum.None
            //   ? Visibility.Visible
            //   : Visibility.Collapsed;

            UModFolderBtn.Visibility = Static.StaticData.CurrentGame == Shared.Enums.GamesEnum.None
                ? Visibility.Collapsed
                : Visibility.Visible;

            this.Title.Text = StaticData.CurrentGame switch
            {
                Shared.Enums.GamesEnum.Oblivion => "Oblivion",
                Shared.Enums.GamesEnum.Fallout => "Fallout",
                _ => "",
            } + " Options";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if(Static.StaticData.CurrentGame == Shared.Enums.GamesEnum.None)
                Navigation.NavigateToPage(PagesEnum.Home, true);
            else
                Navigation.NavigateToPage(PagesEnum.MainMenu, true);
        }

        private void LaunchObmmButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.LaunchModManager();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (StaticData.InstallerInfo.SoftwareUpToDate)
                GeneralHelpers.ShowMessageBox("Software is up to date!");
            else
            {
                if (MessageBoxHelpers.OkCancel("Software update available. Download now?", "Please Update", MessageBoxImage.Information))
                {
                    Application.Current.MainWindow.IsEnabled = false;
                    new UpdateWindow().Show();
                }
            }

        }

        private void ReinstallButton_Click(object sender, RoutedEventArgs e)
        {

            Custom.CustomYesNoMessage yesNoMessage = new Custom.CustomYesNoMessage("This will restore your game to vanilla so you can reinstall the mod pack. Are you sure?");
            yesNoMessage.YesClicked += (s, e) =>
            {
                Thread thread = new Thread(() =>
                {
                    bool success = false;

                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Application.Current.MainWindow.IsEnabled = false;
                            ButtonsArea.Visibility = Visibility.Collapsed;
                            LoadingMessage.Visibility = Visibility.Visible;
                            LoadingMessages.StartCyclingMessages();
                            BackBtn.IsEnabled = false;
                            BackBtn.Opacity = 0.7;
                        }));

                        BackupRestorer backupRestorer = new BackupRestorer();
                        backupRestorer.IsReinstall = true;
                        success = backupRestorer.RestoreBackupForCurrentGame();
                    }
                    catch (Exception ex)
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Logging.Logger.LogException("RestoreOriginalGameData", ex);
                            GeneralHelpers.ShowMessageBox($"Failed to restore game backup! Cannot continue.\n\nError:{ex.Message}");
                        }));
                    }
                    finally
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Application.Current.MainWindow.IsEnabled = true;
                            ButtonsArea.Visibility = Visibility.Visible;
                            LoadingMessage.Visibility = Visibility.Collapsed;
                            LoadingMessages.StopCyclingMessages();
                            BackBtn.IsEnabled = true;
                            BackBtn.Opacity = 1;

                            if (success)
                            {
                                GeneralHelpers.ShowMessageBox($"Game restored.\n\nYou can now perform a clean reinstall.");
                                Navigation.NavigateToPage(Enums.PagesEnum.GameFolderSelect, true);
                            }
                        }));
                    }
                });
                thread.Start();

            };
            yesNoMessage.ShowDialog();
        }

       

        private void ErrorLogsBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Logging.Logger.ErrorLogFolderPath);
        }

        private void UModFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcessHelpers.OpenUModFolder();
        }

    }
}
