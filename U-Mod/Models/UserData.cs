using U_Mod.Models;

namespace U_Mod.Games.Oblivion.Models
{
    public class UserData : UserDataBase
    {
        public bool On4GbRamPatch { get; set; }
        public bool OnModManagerPage { get; set; }
        public bool OnFinalPage { get; set; }
    }
}

namespace U_Mod.Games.Fallout.Models
{
    public class UserData : UserDataBase
    {
        public bool On4GbRamPatch { get; set; }
        public bool OnModManagerPage { get; set; }
        public bool OnFinalPage { get; set; }
    }
}