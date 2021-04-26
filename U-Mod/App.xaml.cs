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
using U_Mod.Shared.Models;
using U_Mod.Static;

namespace U_Mod
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
#if DEV || DEBUG
            //StaticData.MasterList = Helpers.FileHelpers.LoadFile<MasterList>("masterList.json");
            StaticData.MasterList = await Helpers.HttpHelpers.Fetch<MasterList>(Static.Constants.MasterListLink, null);
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


        private async void FetchAppVersionAndMasterList()
        {
            StaticData.InstallerInfo = await HttpHelpers.Fetch<SoftwareVersion>(Constants.SoftwareVersionLink, null);

#if DEV
            await GetMasterList();
#else
            if (StaticData.InstallerInfo.Version == Constants.DefaultSoftwareVersion)
            {
                Helpers.GeneralHelpers.ShowMessageBox("This software requires an internet connection to work.");
                Current.Shutdown();
                return;
            }

            if (!StaticData.InstallerInfo.SoftwareUpToDate)
            {
                if (MessageBoxHelpers.OkCancel("Software update available. Download now?", "Please Update", MessageBoxImage.Information))
                {
                    Application.Current.MainWindow.IsEnabled = false;
                    new UpdateWindow().Show();
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
