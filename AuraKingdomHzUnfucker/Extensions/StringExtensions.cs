using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AuraKingdomHzUnfucker.Extensions
{
    internal static partial class StringExtensions
    {
        internal static string? RemoveSpecialCharacters(this string str)
        {
            if (str == null)
                return null;
            return RemoveSpecialCharacters().Replace(str, "");
        }

        internal static byte[] ToLPTStr(this string str)
        {
            var lptArray = new byte[str.Length + 1];

            var index = 0;
            foreach (char c in str.ToCharArray())
                lptArray[index++] = Convert.ToByte(c);

            lptArray[index] = Convert.ToByte('\0');

            return lptArray;
        }

        [GeneratedRegex("[^a-zA-Z0-9_]+", RegexOptions.Compiled)]
        private static partial Regex RemoveSpecialCharacters();
    }
}
