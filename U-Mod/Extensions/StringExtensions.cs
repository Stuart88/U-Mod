using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U_Mod.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveNumbers(this string s)
        {
            return string.Join("", s.Where(c => !char.IsDigit(c)));
        }

        public static string LettersAndSpacesOnly(this string s)
        {
            return string.Join("", s.Where(c => char.IsLetter(c) || c == ' '));
        }

        /// <summary>
        /// Trims whitespace and remoces spaces. This ensures the start of the string is always of the form "thisIsTheString=" (as opposed to "thisIsTheString =")
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToIniEditString(this string s)
        {
            return s.Trim().Replace(" ", "");
        }
    }
}
