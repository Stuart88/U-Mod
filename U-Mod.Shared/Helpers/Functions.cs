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

        public static int GetIdFromToken(string token)
        {
            string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] tokenData = decodedToken.Split(':');

            if (tokenData.Length < 3)
                return -1;

            if (!int.TryParse(tokenData[0], out int id))
                return -1;

            return id;
        }

        public static List<T> GetEnumsAsList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
