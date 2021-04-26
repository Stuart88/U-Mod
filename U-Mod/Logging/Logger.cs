using System;
using System.IO;
using System.Windows;
using U_Mod.Static;
namespace U_Mod.Logging
{
    public static class Logger
    {
        public static string ErrorLogFolderPath => Path.Combine(Constants.AppDataFolder, "Logs");
        private static string ErrorLogFilePath => Path.Combine(ErrorLogFolderPath, $"ErrorLog-{DateTime.Now.Date:yyyy-MMM-dd}.txt");
        private static readonly object ThreadLock = new object();
        private static int _retries = 0;

        public static void LogException(string processName, Exception e)
        {
            try
            {
                lock (ThreadLock)
                {

                    string log = $"{DateTime.Now:F}\nEXCEPTION:\nGame: {Helpers.GeneralHelpers.GetGameName()}\nFunction: {processName}\nMessage:{Shared.Helpers.StringHelpers.ErrorMessage(e)}\nStack Trace:{e.StackTrace}\n\n";

                    if (!Directory.Exists(ErrorLogFolderPath))
                        _ = Directory.CreateDirectory(ErrorLogFolderPath);

                    using StreamWriter sw = File.AppendText(ErrorLogFilePath);
                    sw.WriteLine(log);
                }

                _retries = 0;

            }
            catch (Exception ex)
            {
                if (_retries++ > 2)
                {
                    _retries = 0;
                    return;
                }
                LogException("Error in LogException?!", ex);
            }
        }

        public static void LogUnhandledException(string processName, Exception e)
        {
            LogException(processName, e);
            
            Helpers.GeneralHelpers.ReEnableAntiPiracy();

            Helpers.GeneralHelpers.ShowExceptionMessageBox("An unhandled exception occurred. This has been logged, and to protect the state of your data the software will now close.", e);

            Application.Current.Shutdown();
        }
    }
}
