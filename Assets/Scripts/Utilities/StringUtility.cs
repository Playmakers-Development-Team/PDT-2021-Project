using System.Text.RegularExpressions;

namespace Utilities
{
    public static class StringUtility
    {
        /// <summary>
        ///     <p>Put spaces between uppercase letters.</p>
        ///     <p>The string "MyEnum" will be turned into "my enum".</p>
        /// </summary>
        /// <param name="o">A string or any object with a valid <c>ToString</c> method.</param>
        /// <returns>The converted string. May call <c>ToString</c> if parameter is an object.</returns>
        public static string UppercaseToReadable(object o) =>
            Regex.Replace(o.ToString(), @"([A-Z])", " $0").Substring(1);
    }
}
