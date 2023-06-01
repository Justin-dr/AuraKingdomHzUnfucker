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
        internal static string RemoveSpecialCharacters(this string str)
        {
            return RemoveSpecialCharacters().Replace(str, "");
        }

        [GeneratedRegex("[^a-zA-Z0-9_]+", RegexOptions.Compiled)]
        private static partial Regex RemoveSpecialCharacters();
    }
}
