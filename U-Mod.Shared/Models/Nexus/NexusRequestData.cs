using System;
using System.Collections.Generic;
using System.Text;

namespace U_Mod.Shared.Models.Nexus
{
    public class NexusRequestData
    {
        public Guid id { get; set; }
        public string token { get; set; }
        public int protocol { get; set; }
    }
}
