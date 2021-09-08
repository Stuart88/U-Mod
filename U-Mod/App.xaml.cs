using System;
using System.Threading.Tasks;
using System.Windows;
using U_Mod.Helpers;
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
                Current.Shutdown();
                return;
            }

            SetupUnhandledExceptionHandling();

            StaticData.SystemInfo = SystemHelper.GetSystemInfo();
        }

        private void SetupUnhandledExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.ExceptionObject as Exception);
            Dispatcher.UnhandledException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.Exception);
            Application.Current.DispatcherUnhandledException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.Exception);
            TaskScheduler.UnobservedTaskException += (s, e) => Logging.Logger.LogUnhandledException("AppDomain.CurrentDomain.UnhandledException", e.Exception);
        }
    }
}