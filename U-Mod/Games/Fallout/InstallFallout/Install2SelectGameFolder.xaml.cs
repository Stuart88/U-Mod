using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using U_Mod.Enums;
using U_Mod.Helpers;
using AmgShared.Helpers;
using Path = System.IO.Path;

namespace U_Mod.Games.Fallout.Pages.InstallFallout
{
    /// <summary>
    /// Interaction logic for Install2.xaml
    /// </summary>
    public partial class Install2SelectGameFolder : UserControl
    {
        #region Private Properties

        private string SelectedGameFolder { get; set; }

        private bool ShowingCWarning { get; set; }

        #endregion Private Properties

        #region Public Constructors

        public Install2SelectGameFolder()
        {
            InitializeComponent();

            this.SelectedGameFolder = Static.StaticData.AppData.FalloutGameFolder;
            this.SelectedFolderText.Text = this.SelectedGameFolder.LeftTrimToLength(40);
            SelectedFolderText.Focus();
            SelectedFolderText.CaretIndex = int.MaxValue;
            this.Focus();

            this.DataContext = this;
        }

        #endregion Public Constructors

        #region Private Methods

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ShowingCWarning)
            {
                HideCDriveWarning();
            }
            else
            {
                ResetWarningText();
                Navigation.NavigateToPage(PagesEnum.FalloutMainMenu);
            }
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

//#if RELEASE || BETA
//            string drive = Path.GetPathRoot(this.SelectedGameFolder);
//            DriveInfo driveInfo = new DriveInfo(drive);
//            if (driveInfo.AvailableFreeSpace < 60E9)
//            {
//                WarningText.Text = "At least 60GB free space required on selected drive!";
//                return;
//            }
//#endif

            // Selected folder exists. Now check for and create AfterMarketGames folder

            DirectoryInfo directory = new DirectoryInfo(this.SelectedGameFolder);

            if (!Directory.Exists(Path.Combine(this.SelectedGameFolder, Static.Constants.AfterMarketGames)))
            {
                directory.CreateSubdirectory(Static.Constants.AfterMarketGames);
            }

            // Now check if user has selected C drive, and warn if this is the first time they've done it

            if (Path.GetPathRoot(this.SelectedGameFolder) == "C:\\" && !Static.StaticData.UserDataStore.CurrentUserData.IgnoredCDrivecheck)
            {
                ShowCDriveWarning();
            }
            else
            {
                FileCheck2();
            }
        }

        /// <summary>
        /// Second stage of file and folder checking. Checks if various required files exist. If not, a warning appears.
        /// If files do exist, user is automatically taken to the next page of the installation process
        /// </summary>
        private void FileCheck2()
        {
            if (this.ShowingCWarning)
            {
                Static.StaticData.UserDataStore.CurrentUserData.IgnoredCDrivecheck = true;
            }
            
            // Check DLC Content

            Static.StaticData.UserDataStore.CurrentUserData.HasAllDlc = HasFullDlcList();

            string[] requiredFiles =
            {
                Path.Combine(this.SelectedGameFolder,"Fallout3.exe"),
                Path.Combine(this.SelectedGameFolder, "Data", "Fallout3.esm"),
            };
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

            // Finally, add ini files
            bool inisOk = CopyCustomIniFiles();
            
            if (inisOk)
            {
                Static.StaticData.SaveAppData();
                Navigation.NavigateToPage(PagesEnum.FalloutInstall3PcSpecs);
            }

        }

        private bool CopyCustomIniFiles()
        {
            try
            {
                FileInfo falloutDefault = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Fallout/IniFiles/Fallout_default.ini"));
                falloutDefault.CopyTo(Path.Combine(FileHelpers.GetGameFolder(), "Fallout_default.ini"), overwrite: true);

                FileInfo falloutIni = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Fallout/IniFiles/Fallout.ini"));
                falloutIni.CopyTo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout3", "Fallout.ini"), overwrite: true);

                FileInfo falloutPrefs = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "Assets/Fallout/IniFiles/FalloutPrefs.ini"));
                falloutPrefs.CopyTo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Fallout3", "FalloutPrefs.ini"), overwrite: true);

                return true;
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("CopyCustomIniFiles (Fallout)", e);
                GeneralHelpers.ShowExceptionMessageBox(e);
                return false;
            }
        }

        private bool HasFullDlcList()
        {
            string dataPath = Path.Combine(this.SelectedGameFolder, "Data");
            string[] dlcFiles =
            {
                "Anchorage.esm",
                "BrokenSteel.esm",
                "PointLookout.esm",
                "ThePitt.esm",
                "Zeta.esm",
            };
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

        private void HideCDriveWarning()
        {
            FilePickerGrid.Visibility = Visibility.Visible;
            CDriveWarnGrid.Visibility = Visibility.Collapsed;
            NextButton.Visibility = Visibility.Visible;
            IgnoreButton.Visibility = Visibility.Collapsed;
            this.ShowingCWarning = false;
        }

        private void IgnoreButton_Click(object sender, RoutedEventArgs e)
        {
            FileCheck2();
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
                Static.StaticData.AppData.FalloutGameFolder = dialog.SelectedPath;
                this.Focus();
                ResetWarningText();
            }
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            HideCDriveWarning();
            ResetWarningText();
            Navigation.NavigateToPage(PagesEnum.FalloutMainMenu, true);
        }

        private void ResetWarningText()
        {
            WarningText.Text = "";
        }

        private void SelectedFolderLabel_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog();
        }
        
        private void ShowCDriveWarning()
        {
            FilePickerGrid.Visibility = Visibility.Collapsed;
            CDriveWarnGrid.Visibility = Visibility.Visible;
            NextButton.Visibility = Visibility.Collapsed;
            IgnoreButton.Visibility = Visibility.Visible;
            this.ShowingCWarning = true;
        }

#endregion Private Methods
    }
}