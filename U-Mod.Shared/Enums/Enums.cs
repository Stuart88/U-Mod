using System;
using System.Collections.Generic;
using System.Text;

namespace U_Mod.Shared.Enums
{
    public enum MembershipType
    {
        None,
        Basic_Month,
        Basic_HalfYear,
        Basic_Year,
        Premium_Month,
        Premium_HalfYear,
        Premium_Year,
        Lifetime,
    }

    [Flags]
    public enum InstallProfileEnum
    {
        AllDlc = 1 << 0,
        Steam = 1 << 1,
        NonSteam = 1 << 4,
        NoDlc = 1 << 5,
        NoData = 1 << 6,
        NoSteamTag = Steam | NonSteam,
        NoDlcTag = AllDlc | NoDlc,
    }

    public enum GamesEnum
    {
        Oblivion,
        Fallout,
        NewVegas,
        Unknown,
        None
    }

    public enum ModState
    {
        Install,
        Update,
        Play
    }
}
