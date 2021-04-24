using AMGWebsite.Shared.Enums;
using U_Mod.Enums;
using U_Mod.Extensions;
using U_Mod.Games.Oblivion.Models;
using U_Mod.Helpers;
using U_Mod.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using UserControl = System.Windows.Controls.UserControl;

namespace U_Mod.Pages.BaseClasses
{
    public partial class MainMenuBase : UserControl
    {
        #region Internal Fields

        internal System.Windows.Controls.Button ActionButton;

        internal System.Windows.Controls.Button OptionsButton;

        #endregion Internal Fields

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

        public MainMenuBase()
        {
           
        }

        internal void InitComponentState()
        {
            SetModState();
            AssignActionButtonStyle();
        }

        #endregion Public Constructors

        #region Public Methods

        public void PlayGame()
        {
            try
            {
                switch (Static.StaticData.CurrentGame)
                {
                    case GamesEnum.Oblivion:
                        if (CheckSteam())
                        {
                            IniFileEditor.EditIniFilesForGame();
                            RunAntiPiracy(false);
                            Tools.LaunchGame();
                            WhileGameRunning();
                        }
                        break;

                    case GamesEnum.Fallout:
                        if (CheckSteam())
                        {
                            Tools.LaunchGame();
                            WhileGameRunning();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                GeneralHelpers.ShowMessageBox($"Failed to launch {GeneralHelpers.GetGameName()}!\n\nError: {AmgShared.Helpers.StringHelpers.ErrorMessage(e)}");
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
            ActionButton.Style = (Static.StaticData.CurrentGame, this.ModState) switch
            {
                // OBLIVION
                (GamesEnum.Oblivion, ModState.Install) => Application.Current.Resources["MenuInstallButton"] as Style,
                (GamesEnum.Oblivion, ModState.Update) => Application.Current.Resources["MenuUpdateButton"] as Style,
                (GamesEnum.Oblivion, ModState.Play) => Application.Current.Resources["MenuPlayButton"] as Style,
                // FALLOUT
                (GamesEnum.Fallout, ModState.Install) => Application.Current.Resources["MenuInstallButtonFallout"] as Style,
                (GamesEnum.Fallout, ModState.Update) => Application.Current.Resources["MenuUpdateButtonFallout"] as Style,
                (GamesEnum.Fallout, ModState.Play) => Application.Current.Resources["MenuPlayButtonFallout"] as Style,
                _ => throw new NotImplementedException(),
            };

            ActionButton.Content = this.ModState switch
            {
                ModState.Install => "Install",
                ModState.Update => "Update",
                ModState.Play => "Play",
                _ => throw new NotImplementedException(),

            };

        }

        private bool CheckSteam()
        {
            if (!Static.StaticData.UserDataStore.OblivionUserData.IsSteamGame)
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

                    OblivionAntiPiracyTool antiPiracyTool = new OblivionAntiPiracyTool();
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
                        GeneralHelpers.ShowMessageBox($"Game init failed at {currentFile}\n\nError: {AmgShared.Helpers.StringHelpers.ErrorMessage(e)}");
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
#if !DEV
                this.OptionsButton.IsEnabled = false;
                this.OptionsButton.Opacity = 0.7;
#endif
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
            ActionButton.IsEnabled = false;
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
                            ActionButton.IsEnabled = true;
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

        public virtual void ActionButton_Click(object sender, RoutedEventArgs e) { }
        public virtual void OptionsButton_Click(object sender, RoutedEventArgs e) { }
        #endregion Private Methods
    }
}