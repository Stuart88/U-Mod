using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using U_Mod.Enums;

namespace U_Mod.Helpers
{
    public static class SystemHelper
    {
        #region Public Methods

        public static GraphicsCard GetGraphicsCardType()
        {
            ManagementObjectSearcher searcher = new
                ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

            foreach (var o in searcher.Get())
            {
                var obj = (ManagementObject)o;
                foreach (PropertyData prop in obj.Properties)
                {
                    if (prop.Name == "Description")
                    {
                        string gc = prop.Value.ToString()?.ToUpper();

                        return gc switch
                        {
                            { } s when s.Contains("INTEL") => GraphicsCard.Intel,
                            { } s when s.Contains("AMD") => GraphicsCard.Amd,
                            { } s when s.Contains("NVIDIA") => GraphicsCard.NVidia,
                            _ => GraphicsCard.Unknown
                        };
                    }
                }
            }

            return GraphicsCard.Unknown;
        }

        public static Rectangle GetScreenResolution()
        {
            return System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        }

        public static ScreenAspectRatio GetScreenAspectRatio()
        {
            var res = GetScreenResolution();
            int gcd = GetGreatestCommonDivisor(res.Width, res.Height);

            (int w, int h) ratio = (res.Width / gcd, res.Height / gcd);

            return ratio switch
            {
                (4, 3) => ScreenAspectRatio._4_3,
                (16, 9) => ScreenAspectRatio._16_9,
                (16, 10) => ScreenAspectRatio._16_10,
                _ => ScreenAspectRatio.Unknown
            };
        }

        /// <summary>
        /// Gets CPU serial number
        /// </summary>
        /// <returns></returns>
        public static string GetMachineId()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["processorID"].Value.ToString();
                break;
            }

            return cpuInfo;
        }

        private static int GetGreatestCommonDivisor(int a, int b)
        {
            return b == 0 ? a : GetGreatestCommonDivisor(b, a % b);
        }

        public static Models.SystemInfo GetSystemInfo()
        {
            Models.SystemInfo systemInfo = new Models.SystemInfo();

            // Get RAM info
            var memorielines = GetWmicOutput("OS get FreePhysicalMemory,TotalVisibleMemorySize /Value").Split("\n");

            string freeMem = memorielines[0].Split("=", StringSplitOptions.RemoveEmptyEntries)[1].Replace('\r', 'n');
            string totalMem = memorielines[1].Split("=", StringSplitOptions.RemoveEmptyEntries)[1].Replace('\r', 'n'); ;

            if (double.TryParse(freeMem, out double dFreeMem))
                systemInfo.FreeMemory = $"{Math.Truncate(dFreeMem / 1000000):F0}GB";
            if (double.TryParse(totalMem, out double dTotalMem))
                systemInfo.TotalMemory = $"{Math.Truncate(dTotalMem / 1000000):F0}GB";

            // Get CPU info
            var cpuLines = GetWmicOutput("CPU get Name,LoadPercentage /Value").Split("\n");

            systemInfo.CpuUse = cpuLines[0].Split("=", StringSplitOptions.RemoveEmptyEntries)[1];
            systemInfo.CpuName = cpuLines[1].Split("=", StringSplitOptions.RemoveEmptyEntries)[1];

            // Get GPU info

            List<string> gpuInfos = new List<string>();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (var obj in searcher.Get())
            {
                if (obj is ManagementObject mo)
                {
                    PropertyData currentBitsPerPixel = mo.Properties["CurrentBitsPerPixel"];
                    PropertyData description = mo.Properties["Description"];
                    if (currentBitsPerPixel.Value != null)
                        gpuInfos.Add(description.Value.ToString());
                }
            }

            systemInfo.GpuNames = string.Join('\n', gpuInfos);

            // Get Drive info

            List<string> driveInfos = new List<string>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                Console.WriteLine("Drive {0}", d.Name);
                Console.WriteLine("  Drive type: {0}", d.DriveType);
                if (d.IsReady)
                {
                    driveInfos.Add($"{d.RootDirectory.FullName?.Replace("\\", "")}  {(d.AvailableFreeSpace / 1000000000):F0} / {(d.TotalSize / 1000000000):F0}GB");
                }
            }

            systemInfo.DriveInfo = string.Join('\n', driveInfos);

            //Get RAM type

            systemInfo.RamType = GetRamType();

            return systemInfo;
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetRamType()
        {
            int type = 0;

            ConnectionOptions connection = new ConnectionOptions();
            connection.Impersonation = ImpersonationLevel.Impersonate;
            ManagementScope scope = new ManagementScope(@"\\.\root\CIMV2", connection);
            scope.Connect();
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_PhysicalMemory");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
            foreach (var o in searcher.Get())
            {
                var queryObj = (ManagementObject)o;
                type = Convert.ToInt32(queryObj["MemoryType"]);
                Debug.WriteLine(type);
            }

            string outValue;

            switch (type)
            {
                case 0x0: outValue = ""; break;
                case 0x1: outValue = ""; break;
                case 0x2: outValue = "DRAM"; break;
                case 0x3: outValue = "Synchronous DRAM"; break;
                case 0x4: outValue = "Cache DRAM"; break;
                case 0x5: outValue = "EDO"; break;
                case 0x6: outValue = "EDRAM"; break;
                case 0x7: outValue = "VRAM"; break;
                case 0x8: outValue = "SRAM"; break;
                case 0x9: outValue = "RAM"; break;
                case 0xa: outValue = "ROM"; break;
                case 0xb: outValue = "Flash"; break;
                case 0xc: outValue = "EEPROM"; break;
                case 0xd: outValue = "FEPROM"; break;
                case 0xe: outValue = "EPROM"; break;
                case 0xf: outValue = "CDRAM"; break;
                case 0x10: outValue = "3DRAM"; break;
                case 0x11: outValue = "SDRAM"; break;
                case 0x12: outValue = "SGRAM"; break;
                case 0x13: outValue = "RDRAM"; break;
                case 0x14: outValue = "DDR"; break;
                case 0x15: outValue = "DDR2"; break;
                case 0x16: outValue = "DDR2 FB-DIMM"; break;
                case 0x17: outValue = "Undefined 23"; break;
                case 0x18: outValue = "DDR3"; break;
                case 0x19: outValue = "FBD2"; break;
                case 0x1a: outValue = "DDR4"; break;
                default: outValue = ""; break;
            }

            return outValue;
        }

        private static string GetWmicOutput(string query, bool redirectStandardOutput = true)
        {
            ProcessStartInfo info = new ProcessStartInfo("wmic")
            {
                CreateNoWindow = true,
                Arguments = query,
                RedirectStandardOutput = redirectStandardOutput
            };

            using var process = Process.Start(info);
            string output = process?.StandardOutput.ReadToEnd();
            return output?.Trim() ?? "";
        }

        #endregion Private Methods
    }
}