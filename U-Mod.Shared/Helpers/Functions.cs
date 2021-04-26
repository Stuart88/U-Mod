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

        public static string GetNameString(this MembershipType membershipType)
        {
            return membershipType switch
            {
                MembershipType.None => "None",
                MembershipType.Basic_Month => "Basic - 1 Month",
                MembershipType.Basic_HalfYear => "Basic - Half Year",
                MembershipType.Basic_Year => "Basic - Full Year",
                MembershipType.Premium_Month => "Premium - 1 Month",
                MembershipType.Premium_HalfYear => "Premium - Half Year",
                MembershipType.Premium_Year => "Premium - Full Year",
                MembershipType.Lifetime => "Lifetime",
                _ => "None",
            };
        }

        public static List<T> GetEnumsAsList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
