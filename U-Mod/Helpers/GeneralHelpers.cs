using System;
using System.Windows;
using U_Mod.Enums;
using U_Mod.Helpers.GameSpecific;
using U_Mod.Models;
using U_Mod.Shared.Constants;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Helpers;

namespace U_Mod.Helpers
{
    public static class GeneralHelpers
    {
        #region Public Methods

        public static string GetGameName()
        {
            return Static.StaticData.CurrentGame switch
            {
                GamesEnum.Oblivion => Constants.GameNameOblivion,
                GamesEnum.Fallout => Constants.GameNameFallout3,
                GamesEnum.NewVegas => Constants.GameNameNewVegas,
                GamesEnum.Skyrim => Constants.GameNameSkyrim,
                _ => "",
            };
        }

        public static void ShowExceptionMessageBox(Exception e)
        {
            string error = Shared.Helpers.StringHelpers.ErrorMessage(e);

            ShowMessageBox(error);
        }

        public static void ShowExceptionMessageBox(string openingMessage, Exception e)
        {
            string error = Shared.Helpers.StringHelpers.ErrorMessage(e);

            ShowMessageBox($"{openingMessage}\n\nError: {error}");
        }
        
        public static void ShowMessageBox(string message)
        {
            Application.Current.MainWindow.IsEnabled = true;
            Custom.CustomMessageWindow msg = new Custom.CustomMessageWindow(message);
            msg.ShowDialog();
        }

        public static void ReEnableAntiPiracy()
        {
            string errorMsg = "";
            string function = "";
            try
            {
                OblivionTools antiPiracyTool = new OblivionTools();
                function = "EnableAntiPiracyMeasures";
                antiPiracyTool.EnableAntiPiracyMeasures(out errorMsg);
                function = "ObseIniFileEnableAntiPiracyEdit";
                antiPiracyTool.ObseIniFileEnableAntiPiracyEdit(out errorMsg);
            }
            catch (Exception ex)
            {
                Logging.Logger.LogException($"ReEnabledAntiPiracy at {function}, filepPath: {errorMsg}", ex);
            }
        }

        #endregion Public Methods
    }
}