using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U_Mod.Shared.Helpers
{
    public static class StringHelpers
    {
        public static string DictToQueryString(Dictionary<string, string> dict)
        {
            if (dict == null)
                return "";

            string searchTerms = "";
            foreach (KeyValuePair<string, string> t in dict)
            {
                searchTerms += $"{t.Key}={t.Value}&";
            }

            return searchTerms;
        }

        public static string TrimToLength(this string s, int length)
        {
            return s.Length > length
                ? s.Substring(0, length) + "... "
                : s;
        }
        public static string LeftTrimToLength(this string s, int length)
        {
            return s.Length > length
                ? "..." + s.Substring(s.Length - length, length)
                : s;
        }
        public static string ErrorMessage(Exception e)
        {
            return e.Message
                   + (e.InnerException != null ? e.InnerException.Message : "")
                   + (e.InnerException?.InnerException != null ? e.InnerException.InnerException.Message : "");
        }

        public static string ReplaceLastOccurrenceOf(this string s, string find, string replace)
        {
            int place = s.LastIndexOf(find, StringComparison.Ordinal);

            if (place == -1)
                return s;

            string result = s.Remove(place, find.Length).Insert(place, replace);
            return result;
        }


    }
}
