using System;
using System.Collections.Generic;
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
using U_Mod.Helpers;
using U_Mod.Models;

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
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(GeneralHelpers.GetMainMenuPageEnumForGame(), true);
        }

        private void LaunchObmmButton_Click(object sender, RoutedEventArgs e)
        {
            Tools.LaunchModManager();
        }


        private void ReinstallButton_Click(object sender, RoutedEventArgs e)
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
                            Application.Current.MainWindow.IsEnabled = false;
                            ButtonsArea.Visibility = Visibility.Collapsed;
                            LoadingMessage.Visibility = Visibility.Visible;
                            BackBtn.IsEnabled = false;
                            BackBtn.Opacity = 0.7;
                        }));

                        BackupRestorer backupRestorer = new BackupRestorer();
                        backupRestorer.IsReinstall = true;
                        _ = backupRestorer.RestoreBackupForCurrentGame();

                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            GeneralHelpers.ShowMessageBox($"Game restored.\n\nYou can now perform a clean reinstall.");
                            Navigation.NavigateToPage(Enums.PagesEnum.GameFolderSelect, true);
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
                            BackBtn.IsEnabled = true;
                            BackBtn.Opacity = 1;
                        }));
                    }
                    finally
                    {
                        Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                        {
                            Application.Current.MainWindow.IsEnabled = true;

                        }));
                    }
                });
                thread.Start();

            };
            yesNoMessage.ShowDialog();
        }
    }
}
