using AmgShared.Models;
using AMGWebsite.Shared.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using U_Mod.Helpers;
using U_Mod.Models;
using U_Mod.Pages;
using U_Mod.Static;

namespace U_Mod
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        private string Version { get; set; } = "1.0.0";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (ProcessHelpers.ProcessRunning("U-Mod"))
            {
                //MessageBox.Show("GameHub is already running!");
                Current.Shutdown();
                return;
            }

            SetupUnhandledExceptionHandling();

            StaticData.SystemInfo = SystemHelper.GetSystemInfo();

            FetchAppVersionAndMasterList();
        }

        public async Task GetMasterList()
        {
#if DEV
            //StaticData.MasterList = Helpers.FileHelpers.LoadFile<MasterList>("masterList.json");
            StaticData.MasterList = await Helpers.HttpHelpers.Fetch<MasterList>(Static.Constants.MasterListLink, null);
#elif DEBUG || BETA
            try
            {
                throw new Exception();
                StaticData.MasterList = Helpers.FileHelpers.LoadFile<MasterList>("C:\\Users\\saitk\\Programming\\WPF\\Mod Installer\\Mod Installer\\GameHub\\masterListB.json");
            }
            catch (Exception e)
            {
                StaticData.MasterList = await Helpers.HttpHelpers.Fetch<MasterList>(Static.Constants.MasterListLink, null);
            }
#else
            StaticData.MasterList = await Helpers.HttpHelpers.Fetch<MasterList>(Static.Constants.MasterListLink, null);
#endif

            AssignMasterListOrderValues();
        }
        private void SetupUnhandledExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject as Exception);
            Dispatcher.UnhandledException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.Exception);
            Application.Current.DispatcherUnhandledException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.Exception);
            TaskScheduler.UnobservedTaskException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.Exception);
        }

        public async Task GetInstallerInfo()
        {
            StaticData.KillswitchInfo = await Helpers.HttpHelpers.Fetch<KillswitchInfo>(Constants.OblivionInstallerInfoLink, null);

#if DEBUG || RELEASE
            StaticData.KillswitchInfo.IsBeta = false;
#endif
            //if Version is -1, internet connection must have failed.
            if (StaticData.KillswitchInfo.Version == -1)
            {
                Helpers.GeneralHelpers.ShowMessageBox("This software requires an internet connection to work.");
                Current.Shutdown();
                return;
            }

            StaticData.InstallerInfo = await HttpHelpers.Fetch<SoftwareVersion>(Constants.SoftwareVersionLink, null);
        }


        private async void FetchAppVersionAndMasterList()
        {
            await GetInstallerInfo();

#if DEV
            await GetMasterList();
#else
            if (StaticData.InstallerInfo.Version == Constants.DefaultSoftwareVersion)
            {
                Helpers.GeneralHelpers.ShowMessageBox("This software requires an internet connection to work.");
                Current.Shutdown();
                return;
            }

            if (StaticData.InstallerInfo.Version != this.Version)
            {
                if (MessageBoxHelpers.OkCancel("You must update Gamehub before continuing", "Please Update", MessageBoxImage.Information))
                {
                    new UpdateWindow().Show();
                }
                else
                {
                    Current.Shutdown();
                    return;
                }
            }
            else
            {
                await GetMasterList();
            }
#endif
        }


        public void AssignMasterListOrderValues()
        {
            // TODO: DELETE THIS LATER WHEN ORDER VALUES ARE DONE ON DB

            for (int i = 0; i < Static.StaticData.MasterList.Games.Count; i++)
            {
                for (int j = 0; j < Static.StaticData.MasterList.Games[i].Mods.Count; j++)
                {
                    Static.StaticData.MasterList.Games[i].Mods[j].InstallOrder = j;

                    for (int k = 0; k < Static.StaticData.MasterList.Games[i].Mods[j].Files.Count; k++)
                    {
                        Static.StaticData.MasterList.Games[i].Mods[j].Files[k].InstallOrder = k;

                        for (int l = 0; l < Static.StaticData.MasterList.Games[i].Mods[j].Files[k].Content.Count; l++)
                        {
                            Static.StaticData.MasterList.Games[i].Mods[j].Files[k].Content[l].InstallOrder = l;
                        }
                    }
                }
            }
        }
    }

}
