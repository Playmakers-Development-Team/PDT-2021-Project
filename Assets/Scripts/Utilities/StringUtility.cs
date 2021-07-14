using System.Text.RegularExpressions;

namespace Utilities
{
    public static class StringUtility
    {
        /// <summary>
        /// Put spaces between uppercase letters.
        /// <p>The string "MyEnum" will be turned into "my enum".</p>
        /// </summary>
        /// <param name="o">A string or any object with a valid ToString method</param>
        /// <returns>The converted string. May call ToString if parameter is an object</returns>
        public static string UppercaseToReadable(object o) =>
            Regex.Replace(o.ToString(), @"([A-Z])", " $0").Substring(1);
    }
}
