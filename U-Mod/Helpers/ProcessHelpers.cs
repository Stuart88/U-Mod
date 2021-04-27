using System;
using System.Diagnostics;

namespace U_Mod.Helpers
{
    public static class ProcessHelpers
    {
        public static void OpenInBrowser(string url)
        {
            var sInfo = new System.Diagnostics.ProcessStartInfo(url)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }

        public static void OpenUModFolder()
        {
            Process.Start("explorer.exe", FileHelpers.GetUModFolder());
        }

        public static void OpenGameFolder()
        {
            Process.Start("explorer.exe", FileHelpers.GetGameFolder());
        }

        public static bool ProcessRunning(string processName)
        {
            Process[] pname = Process.GetProcessesByName(processName);
            return pname.Length > 1;
        }

        public static bool ProcessRunningThatStartsWith(string processNameStartsWith)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName.ToLower().StartsWith(processNameStartsWith.ToLower()))
                    return true;
            }
            return false;
        }

        public static Process GetProcessRunningThatStartsWith(string processNameStartsWith)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName.ToLower().StartsWith(processNameStartsWith.ToLower()))
                    return p;
            }
            return null;
        }

        public static bool AnyProcessStartsWith(string processNameStartsWith)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                if (p.ProcessName.ToLower().StartsWith(processNameStartsWith.ToLower()))
                    return true;
            }
            return false;
        }

        public static bool AnyProcessRunningExe(string exeToCheckFor)
        {
            Process[] processes = Process.GetProcesses();
            foreach (var p in processes)
            {
                try
                {
                    if (p.MainModule?.ModuleName == exeToCheckFor)
                        return true;
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Happens if MainModule cannot be accessed
                    continue;
                }
            }
            return false;
        }

        /// <summary>
        /// Steam could be installed anywhere on computer, so safest thing is to find the running steam process
        /// and get its path from that. If steam not running user can just be told to run steam
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool GetSteamDirectoryFromProcesses(out string filePath)
        {
            try
            {
                filePath = string.Empty;

                Process p = GetProcessRunningThatStartsWith("steam");

                if (p?.MainModule != null)
                {
                    filePath = p.MainModule.FileName ?? "";

                    if (filePath.EndsWith("steam.exe"))
                        return true;
                    else
                    {
                        filePath = "";
                        return false;
                    }

                }

                return false;
            }
            catch (Exception e)
            {
                Logging.Logger.LogException("GetSteamDirectoryFromProcesses", e);
                filePath = "";
                return false;
            }
        }
    }
}
