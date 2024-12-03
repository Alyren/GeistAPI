using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeistTools.Tweaks.Core.Extensions
{
    internal static class StringExtensions
    {
        public static string CensorWindowsUsername(this string value)
        {
            var username = Environment.UserName;
            var censoredName = "".PadLeft(username.Length, '*');
            return value.Replace(username, censoredName);
        }
    }
}
