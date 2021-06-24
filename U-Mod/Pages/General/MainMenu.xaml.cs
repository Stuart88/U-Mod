using U_Mod.Shared.Enums;
using U_Mod.Enums;
using U_Mod.Extensions;
using U_Mod.Helpers;
using U_Mod.Pages.BaseClasses;
using System.Linq;
using System.Windows;
using System;
using U_Mod.Models;
using System.Windows.Forms;
using U_Mod.Helpers.GameSpecific;
using System.Windows.Media.Imaging;

namespace U_Mod.Pages.General
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : System.Windows.Controls.UserControl
    {

        #region Private Fields

        private Timer timer;

        private Timer timer2;

        #endregion Private Fields

        #region Internal Properties

        internal ModState ModState { get; set; } = ModState.Install;

        #endregion Internal Properties

        #region Private Properties

        private string GameProcessName => Static.StaticData.CurrentGame switch
        {
            GamesEnum.Oblivion => "oblivion",
            GamesEnum.Fallout => "fallout3",
            _ => throw new Exception("No game name found!")
        };

        #endregion Private Properties

        #region Public Constructors

        public MainMenu()
        {
            InitializeComponent();

            this.MenuTitle.Text = GeneralHelpers.GetGameName();
            this.MenuImage.Source = new BitmapImage(new Uri($"/Assets/Images/{GetMenuImageName()}", UriKind.Relative));

            InitComponentState();
        }

        private string GetMenuImageName()
        {
            return Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => "menu-oblivion.jpg",
                GamesEnum.Fallout => "menu-fallout.jpg",
                GamesEnum.NewVegas => throw new NotImplementedException(),
                GamesEnum.Unknown => throw new NotImplementedException(),
                GamesEnum.None => throw new NotImplementedException(),
            };
        }

        internal void InitComponentState()
        {
            SetModState();
            AssignActionButtonStyle();
        }

        #endregion Public Constructors

        #region Public Methods

        public void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            switch (this.ModState)
            {
                case ModState.Install:
                    if(Static.StaticData.UserDataStore.CurrentUserData.On4GbRamPatch)
                        Navigation.NavigateToPage(PagesEnum.PatchAndModManager, true);
                    else
                        Navigation.NavigateToPage(PagesEnum.GameFolderSelect, true);
                    break;
                case ModState.Play:
                    PlayGame();
                    break;

                case ModState.Update:

                    //Auto select all mods requiring update

                    Static.StaticData.UserDataStore.CurrentUserData.SelectedToInstall = Static.StaticData.MasterList.GetModUpdates()
                        .Select(m => new ModListItem
                        {
                            IsDownloaded = false,
                            IsInstalled = false,
                            IsDirectDownloadOnly = m.Files.All(f => !string.IsNullOrEmpty(f.DirectDownloadUrl)),
                            Mod = m
                        })
                        .ToList();

                    Static.StaticData.UserDataStore.CurrentUserData.IsUpdating = true;
                    Static.StaticData.UserDataStore.CurrentUserData.InstallationComplete = false;

                    if (Static.StaticData.UserDataStore.CurrentUserData.SelectedToInstall.Any(m => !m.Mod.IsEssential || ModHelpers.IsNewMod(m.Mod)))
                    {
                        //Some mods are new and are optional, so go to mod selection list
                        Navigation.NavigateToPage(PagesEnum.ModsSelect);
                    }
                    else
                    {
                        //Nothing to choose from, updates are just updates and/or essential, so go straight to download/process steps.
                        Navigation.NavigateToPage(PagesEnum.ModsSelect);

                        //NOTE both blocks are now the same. Not sure if this will work out or not..
                    }

                    break;
            }
        }
        public void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            Navigation.NavigateToPage(PagesEnum.Options, true);
        }

        public void PlayGame()
        {
            try
            {
                switch (Static.StaticData.CurrentGame)
                {
                    case GamesEnum.Oblivion:
                        ProcessHelpers.TryToLaunchOblivionModManager();
                        break;
                    case GamesEnum.Fallout:
                        ProcessHelpers.TryToLaunchModOrganizer();
                        break;
                }
            }
            catch (Exception e)
            {
                GeneralHelpers.ShowMessageBox($"Failed to launch {GeneralHelpers.GetGameName()}!\n\nError: {Shared.Helpers.StringHelpers.ErrorMessage(e)}");
                Logging.Logger.LogException($"PlayGame ({GeneralHelpers.GetGameName()})", e);
                return;
            }


        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Checks game/mod state and assigns button with one of Install/Update/Play graphics
        /// </summary>
        private void AssignActionButtonStyle()
        {

            ActionButton.Content = this.ModState switch
            {
                ModState.Install => "Install",
                ModState.Update => "Update",
                ModState.Play => "Play",
                _ => throw new NotImplementedException(),
            };

            if(this.ModState == ModState.Play)
            {
                switch (Static.StaticData.CurrentGame)
                {
                    case GamesEnum.Fallout:
                    case GamesEnum.Oblivion:
                        ActionButton.Content = "Launch";
                        ModOrganizerText.Visibility = Visibility.Visible;
                        break;
                }
            }

            ModManagerName.Text = Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => Static.Constants.Obmm,
                _ => Static.Constants.ModOrganizer2,
            };
        }

        private bool CheckSteam()
        {
            if (!Static.StaticData.UserDataStore.CurrentUserData.IsSteamGame)
                return true;

            if (!ProcessHelpers.AnyProcessStartsWith("steamwebhelper") && !ProcessHelpers.AnyProcessStartsWith("steam") && !ProcessHelpers.AnyProcessStartsWith("SteamService"))
            {
                GeneralHelpers.ShowMessageBox("Please open Steam before launching game.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Runs anti-piracy measures
        /// </summary>
        /// <param name="enable">Whether to enable or disbale anti-piracy measures</param>
        private void RunAntiPiracy(bool enable)
        {
            switch (Static.StaticData.CurrentGame)
            {
                case GamesEnum.Oblivion:

                    OblivionTools antiPiracyTool = new OblivionTools();
                    string currentFile = "";
                    try
                    {
                        if (enable)
                        {
                            antiPiracyTool.EnableAntiPiracyMeasures(out currentFile);
                            antiPiracyTool.ObseIniFileEnableAntiPiracyEdit(out currentFile);
                        }
                        else
                        {
                            antiPiracyTool.DisableAntiPiracyMeasures(out currentFile);
                            antiPiracyTool.ObseIniFileDisableAntiPiracyEdit(out currentFile);
                        }
                    }
                    catch (Exception e)
                    {
                        GeneralHelpers.ShowMessageBox($"Game init failed at {currentFile}\n\nError: {Shared.Helpers.StringHelpers.ErrorMessage(e)}");
                        Logging.Logger.LogException("RunAntiPiracy (Oblivion)", e);
                        return;
                    }

                    break;

                case GamesEnum.Fallout:
                    break;
            }
        }

        private void SetModState()
        {
            if (!Static.StaticData.UserDataStore.CurrentUserData.InstallationComplete)
            {
                this.ModState = ModState.Install;
            }
            else if (Static.StaticData.MasterList.HasModUpdates())
            {
                this.ModState = ModState.Update;
            }
            else
            {
                this.ModState = ModState.Play;
            }
        }

        private void WhileGameRunning()
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                System.Windows.Application.Current.MainWindow.IsEnabled = false;
            }));
            ActionButton.IsEnabled = false;
            OptionsButton.IsEnabled = false;
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (s, e) =>
            {
                // Rapid intervals checking for running game processes
                // If no processes running, set new 3s timer. If after 3s there are still no processes, then the game must
                // have been closed, so undo anti-pracy and re-enable the Play button

                if (!ProcessHelpers.ProcessRunningThatStartsWith(GameProcessName))
                {
                    timer.Stop();
                    timer2 = new Timer();
                    timer2.Interval = 3000;
                    timer2.Tick += (ss, ee) =>
                    {
                        if (!ProcessHelpers.ProcessRunningThatStartsWith(GameProcessName))
                        {
                            timer.Stop();
                            timer.Enabled = false;
                            System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                System.Windows.Application.Current.MainWindow.IsEnabled = true;
                            }));
                            ActionButton.IsEnabled = true;
                            OptionsButton.IsEnabled = true;
                            RunAntiPiracy(true);
                        }
                        else
                        {
                            //restart main timer
                            timer.Start();
                        }
                        timer2.Stop();
                        timer2.Enabled = false;
                    };
                    timer2.Start();
                }
            };
            timer.Start();
        }


        #endregion Private Methods

        private void ModManagerInstructionsLink_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Tools.OpenModManagerHelpInBrowser();
        }
    }
}
