using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public static class StringExtensions
    {
        /// <summary>
        /// convert string to Base64 string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// convert from Base64 string to string
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static string FromBase64(this string base64)
        {
            var data = Convert.FromBase64String(base64);
            var str = Encoding.UTF8.GetString(data);
            return str;
        }
    }
}
