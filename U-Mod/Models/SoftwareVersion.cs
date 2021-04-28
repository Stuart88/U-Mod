using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U_Mod.Static;

namespace U_Mod.Models
{
    public class SoftwareVersion
    {
        public string Version { get; set; } = Constants.DefaultSoftwareVersion;
        public string CompareVersion { get; set; } = Shared.Constants.Constants.InstallerVersion;

        public bool SoftwareUpToDate => this.Version == this.CompareVersion;
    }
}
