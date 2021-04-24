namespace U_Mod.Models
{
    public class SystemInfo
    {
        #region Public Properties

        public string CpuName { get; set; } = "Unknown";
        public string CpuUse { get; set; } = "Unknown";
        public string DriveInfo { get; set; } = "Unknown";
        public string FreeMemory { get; set; } = "Unknown";
        public string GpuNames { get; set; } = "Unknown";
        public string RamType { get; set; } = "";
        public string TotalMemory { get; set; } = "Unknown";

        #endregion Public Properties
    }
}