using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SS.Api.helpers.extensions
{
    public static class StringExtensions
    {
        public static string EnsureEndingForwardSlash(this string target) => target.EndsWith("/") ? target : $"{target}/";

        public static string ConvertCamelCaseToMultiWord(this string target) =>
            Regex.Replace(target, "([A-Z])", " $1").Trim().ToLower();


    }
}
