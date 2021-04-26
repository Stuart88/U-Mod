using System;
using System.Collections.Generic;
using System.Text;

namespace U_Mod.Shared.Models.Nexus
{
    public class NexusResponse
    {
        public bool success { get; set; }
        public NexusData data { get; set; }
        public object error { get; set; }
    }


}
