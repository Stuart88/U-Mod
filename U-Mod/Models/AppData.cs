using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U_Mod.Models
{
    public class AppData
    {
        public int SoftwareVersion { get; set; } = 1;
        public string OblivionGameFolder { get; set; } = "";
        public string FalloutGameFolder { get; set; } = "";
        public string FalloutNewVegasGameFolder { get; set; } = "";
        public string NexusLoginToken { get; set; } = "";
        public Guid NexusUuid { get; set; } = Guid.Empty;
    }
}
