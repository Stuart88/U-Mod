using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
            if (Directory.Exists(FileHelpers.GetUModFolder()))
                Process.Start("explorer.exe", FileHelpers.GetUModFolder());
            else
                GeneralHelpers.ShowMessageBox("U-Mod folder does not yet exist. It will be created after selecting your game folder in the install process");
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

        public static void TryToLaunchOblivionModManager()
        {
            string defaultPath = Path.Combine(Static.StaticData.AppData.OblivionGameFolder, "OblivionModManager.exe"); ;
            if (File.Exists(defaultPath))
            {
                Tools.LaunchProcess(defaultPath, "TryToLaunchModOrganizer (defaulPath)", null, true);
                return;
            }

            //OBMM not in default install location, so let's desperately try hunting for the app path

            string pathToExe = "";

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = LocateEXEfromPathVariables("OblivionModManager.exe");

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = GetExePathFromRegistry("Oblivion Mod Manager");

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = GetAppPathFromInstallFolders("OBMM");

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = GetAppPathFromInstallFolders("Oblivion Mod Manager");

            if (!string.IsNullOrEmpty(pathToExe))
                Tools.LaunchProcess(pathToExe, "TryToLaunchOblivionModManager (pathToExe)", null, true);
            else
                GeneralHelpers.ShowMessageBox("Cannot find executable. Please install Oblivion Mod Manager, or if it is already installed, please locate and launch it manually.");
        }

        public static string GetModOrganizerExePath()
        {
            string defaultPath = Path.Combine("C:", "Modding", "MO2", "ModOrganizer.exe");
            if (File.Exists(defaultPath) && false)
            {
                return defaultPath;
            }

            //Mod Organizer not in default install location, so let's desperately try hunting for the app path

            string pathToExe = "";

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = LocateEXEfromPathVariables("ModOrganizer.exe");

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = GetExePathFromRegistry("Mod Organizer");

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = GetAppPathFromInstallFolders("MO2");

            if (string.IsNullOrEmpty(pathToExe))
                pathToExe = GetAppPathFromInstallFolders("Mod Organizer");

            return pathToExe;
        }

        public static void TryToLaunchModOrganizer()
        {
            string defaultPath = Path.Combine("C:", "Modding", "MO2", "ModOrganizer.exe"); ;
            if (File.Exists(defaultPath))
            {
                Tools.LaunchProcess(defaultPath, "TryToLaunchModOrganizer (defaulPath)", null, true);
                return;
            }

            //Mod Organizer not in default install location, so let's desperately try hunting for the app path

            string pathToExe = GetModOrganizerExePath();

            if (!string.IsNullOrEmpty(pathToExe))
                Tools.LaunchProcess(pathToExe, "TryToLaunchModOrganizer (pathToExe)", null, true);
            else
                GeneralHelpers.ShowMessageBox("Cannot find Mod Organizer executable. Please install Mod Organizer, or if it is already installed, please locate Mod Organizer and launch it manually.");
        }

        private static string LocateEXEfromPathVariables(String filename)
        {
            String path = Environment.GetEnvironmentVariable("path");
            String[] folders = path.Split(';');
            foreach (String folder in folders)
            {
                if (File.Exists(folder + filename))
                {
                    return folder + filename;
                }
                else if (File.Exists(folder + "\\" + filename))
                {
                    return folder + "\\" + filename;
                }
            }

            return String.Empty;
        }

        private static string GetExePathFromRegistry(string findByName)
        {
            string displayName;
            string InstallPath;
            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

            //64 bits computer
            RegistryKey key64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey key = key64.OpenSubKey(registryKey);

            if (key != null)
            {
                var keyNames = key.GetSubKeyNames()
                    .Select(keyName => key.OpenSubKey(keyName).GetValue("DisplayName") as string)
                    .Where(k => k != null)
                    .ToList();

                keyNames.Sort();


                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(findByName))
                    {

                        InstallPath = subkey.GetValue("InstallLocation").ToString();

                        return InstallPath; //or displayName

                    }
                }
                key.Close();
            }

            return null;
        }

        /// <summary>
        /// Looks in standard install locations e.g. ProgramFiles
        /// </summary>
        /// <param name="productName"></param>
        /// <returns></returns>
        private static string GetAppPathFromInstallFolders(string productName)
        {
            const string foldersPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Installer\Folders";
            var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

            var subKey = baseKey.OpenSubKey(foldersPath);
            if (subKey == null)
            {
                baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                subKey = baseKey.OpenSubKey(foldersPath);
            }
            string result = subKey != null ? subKey.GetValueNames().FirstOrDefault(kv => kv.Contains(productName)) : "";
            return result;
        }
    }
}
