using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using U_Mod.Shared.Enums;
using U_Mod.Shared.Models;

namespace U_Mod.Shared.Helpers
{
    public static class Functions
    {
        public static List<T> GetEnumsAsList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
