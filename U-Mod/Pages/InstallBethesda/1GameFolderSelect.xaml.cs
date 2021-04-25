using AmgShared.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using U_Mod.Enums;
using U_Mod.Helpers;
using U_Mod.Pages.General;
using Path = System.IO.Path;

namespace U_Mod.Pages.InstallBethesda
{
    /// <summary>
    /// Interaction logic for _1GameFolderSelect.xaml
    /// </summary>
    public partial class _1GameFolderSelect : UserControl
    {
        #region Private Properties

        private string SelectedGameFolder { get; set; }

        #endregion Private Properties

        #region Public Constructors

        public _1GameFolderSelect()
        {
            InitializeComponent();

            this.FolderSelectText_GameName.Text = GeneralHelpers.GetGameName();
            this.SelectedGameFolder = FileHelpers.GetGameFolder();
            this.SelectedFolderText.Text = this.SelectedGameFolder.LeftTrimToLength(40);
            SelectedFolderText.Focus();
            SelectedFolderText.CaretIndex = int.MaxValue;
            this.FullInstallCheck.IsChecked = Static.StaticData.UserDataStore.CurrentUserData.CanFullInstall;
            this.Focus();

            this.DataContext = this;
        }

        #endregion Public Constructors

        #region Private Methods

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            ResetWarningText();

            Navigation.NavigateToPage(PagesEnum.MainMenu);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog();
        }

        /// <summary>
        /// Performs first set of file and folder checks. If user has selected C drive, a warning is shown and the process stops.
        /// If user has not selected C drive, this method continues straight into FileCheck2()
        /// </summary>
        private void FileCheck1()
        {
            // Basic validation

            if (string.IsNullOrEmpty(this.SelectedGameFolder))
            {
                WarningText.Text = "Please first choose a game folder";
                return;
            }

            if (!Directory.Exists(this.SelectedGameFolder))
            {
                WarningText.Text = "Selected folder does not exist!";
                return;
            }

            DirectoryInfo directory = new DirectoryInfo(this.SelectedGameFolder);

            if (!Directory.Exists(Path.Combine(this.SelectedGameFolder, Static.Constants.UMod)))
            {
                directory.CreateSubdirectory(Static.Constants.UMod);
            }

            FileCheck2();
        }

        /// <summary>
        /// Second stage of file and folder checking. Checks if various required files exist. If not, a warning appears.
        /// If files do exist, user is automatically taken to the next page of the installation process
        /// </summary>
        private void FileCheck2()
        {
           
            // Check DLC Content

            Static.StaticData.UserDataStore.CurrentUserData.HasAllDlc = HasFullDlcList();

            string[] requiredFiles = this.GetRequiredFilesForGame();

            string filesNotFoundStr = "";

            foreach (string file in requiredFiles)
            {
                if (!File.Exists(file))
                    filesNotFoundStr += $"{file}\n\n";
            }

            if (!string.IsNullOrEmpty(filesNotFoundStr))
            {
                GeneralHelpers.ShowMessageBox("The following files were not found:\n\n" + filesNotFoundStr + "\n\nEnsure these files exist, then try again");
                return;
            }

            // Now check if it's steam

            Static.StaticData.UserDataStore.CurrentUserData.IsSteamGame = this.SelectedGameFolder.ToLower().Contains(@"steamapps");

            if (!Static.StaticData.UserDataStore.CurrentUserData.IsSteamGame)
            {
                GeneralHelpers.ShowMessageBox("This installer only works for Steam games. U_Mod will be ready for non-Steam in a future update");
                NextButton.IsEnabled = false;
                NextButton.Opacity = 0.6;
                return;
            }

            // If so, add steam.exe to game folder
            if (Static.StaticData.UserDataStore.CurrentUserData.IsSteamGame)
            {
                string copyToPath = Path.Combine(this.SelectedGameFolder, "steam.exe");

                if (!File.Exists(copyToPath))
                {
                    //First check bog standard steam location
                    if (File.Exists(@"C:\Program Files (x86)\Steam\steam.exe"))
                    {
                        FileInfo steamexe = new FileInfo(@"C:\Program Files (x86)\Steam\steam.exe");
                        steamexe.CopyTo(copyToPath);
                    }
                    // else see if steam is running, and if so, try to get its exe path from wherever it is
                    else if (ProcessHelpers.GetSteamDirectoryFromProcesses(out string steamPath))
                    {
                        FileInfo steamexe = new FileInfo(steamPath);
                        steamexe.CopyTo(copyToPath);
                    }
                    // else, couldn't find steam exe path, tell user to run steam and try again, or manually select Steam exe
                    else
                    {
                        GeneralHelpers.ShowMessageBox("Installer cannot detect Steam. Please ensure Steam is running.\n\nIf Steam is already running, you may need to manually select steam.exe from the directory where Steam is installed. You can do this in the next dialog.\n\nYou should look for:\n\n...\\Your Steam Directory\\Steam\\steam.exe");

                        var dialog = new Ookii.Dialogs.Wpf.VistaOpenFileDialog() { InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) };
                        dialog.Multiselect = false;

                        if (dialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
                        {
                            if (dialog.FileName.ToLower().EndsWith("steam.exe"))
                            {
                                FileInfo steamexe = new FileInfo(dialog.FileName);
                                steamexe.CopyTo(copyToPath);
                                MessageBox.Show("Success!");
                            }
                            else
                            {
                                GeneralHelpers.ShowMessageBox("steam.exe not found!");
                                return;
                            }
                        }
                    }
                }
            }

            bool finalStepOk = true;

            // Game specific steps
            if (Static.StaticData.CurrentGame == AMGWebsite.Shared.Enums.GamesEnum.Fallout)
            {
                finalStepOk = Helpers.GameSpecific.FalloutTools.CopyCustomIniFiles();
            }

            if (finalStepOk)
            {
                Static.StaticData.SaveAppData();
                Navigation.NavigateToPage(PagesEnum.ModsSelect, true);
            }
        }

        private string[] GetDlcFilesForGame()
        {
            switch (Static.StaticData.CurrentGame)
            {
                case AMGWebsite.Shared.Enums.GamesEnum.Oblivion:
                    return new string[]
                    {
                        "DLCBattlehornCastle.esp",
                        "DLCFrostcrag.esp",
                        "DLCHorseArmor.esp",
                        "DLCMehrunesRazor.esp",
                        "DLCOrrery.esp",
                        "DLCSpellTomes.esp",
                        "DLCThievesDen.esp",
                        "DLCVileLair.esp",
                    };

                case AMGWebsite.Shared.Enums.GamesEnum.Fallout:
                    return new string[]
                    {
                        "Anchorage.esm",
                        "BrokenSteel.esm",
                        "PointLookout.esm",
                        "ThePitt.esm",
                        "Zeta.esm",
                    };

                default:
                    return new string[0];
            }
        }

        private string[] GetRequiredFilesForGame()
        {
            switch (Static.StaticData.CurrentGame)
            {
                case AMGWebsite.Shared.Enums.GamesEnum.Oblivion:
                    return new string[]
                    {
                        Path.Combine(this.SelectedGameFolder,"Oblivion.exe"),
                        Path.Combine(this.SelectedGameFolder, "Data", "DLCShiveringisles.esp"),
                        Path.Combine(this.SelectedGameFolder, "Data", "Knights.esp"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Oblivion", "BlendSettings.ini"),
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Oblivion", "Oblivion.ini"),
                    };

                case AMGWebsite.Shared.Enums.GamesEnum.Fallout:
                    return new string[]
                    {
                        Path.Combine(this.SelectedGameFolder,"Fallout3.exe"),
                        Path.Combine(this.SelectedGameFolder, "Data", "Fallout3.esm"),
                    };

                default:
                    return new string[0];
            }
        }

        private bool HasFullDlcList()
        {
            string dataPath = Path.Combine(this.SelectedGameFolder, "Data");
            string[] dlcFiles = GetDlcFilesForGame();
            if (Directory.Exists(dataPath))
            {
                foreach (string f in dlcFiles)
                {
                    if (!File.Exists(Path.Combine(dataPath, f)))
                        return false;
                }

                return true;
            }

            return false;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            ResetWarningText();

            FileCheck1();
        }

        private void OpenFileDialog()
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
            {
                NextButton.IsEnabled = true;
                NextButton.Opacity = 1;

                this.SelectedGameFolder = dialog.SelectedPath;
                SelectedFolderText.Text = dialog.SelectedPath.LeftTrimToLength(40);
                FileHelpers.SetGameFolder(dialog.SelectedPath);
                this.Focus();
                ResetWarningText();
            }
        }


        private void ResetWarningText()
        {
            WarningText.Text = "";
        }

        private void SelectedFolderLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog();
        }

       
        private void FullInstallCheck_Checked(object sender, RoutedEventArgs e)
        {
            Static.StaticData.UserDataStore.CurrentUserData.CanFullInstall = this.FullInstallCheck.IsChecked ?? false;
            Static.StaticData.SaveAppData();
        }

        #endregion Private Methods

        private SystemInfoView SysInfoWindow { get; set; } = new SystemInfoView();

        private void SystemInfoLink_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.Windows.OfType<SystemInfoView>().Any(w => w is SystemInfoView))
            {
                Application.Current.Windows.OfType<SystemInfoView>().First(w => w is SystemInfoView).Visibility = Visibility.Visible;
                Application.Current.Windows.OfType<SystemInfoView>().First(w => w is SystemInfoView).Topmost = true;
            }
            else
            {
                this.SysInfoWindow = new SystemInfoView();
                SysInfoWindow.Show();
            };
           

        }
    }
}