using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CommonLib
{
    public static class HashFunction
    {
        public static string Get(string secret)
        {
            var config = new ConfigurationSettings();

            var date = DateTime.Now;
            var login = string.Empty;
            var uuid = config.GetUUID();            

            return Get(uuid, secret, date, login);
        }

        public static string Get(string uuid, string secretWord, DateTime date, string login)
        {            
            var dateStr = date.ToString("MMddyyyyHH");
            var str = string.Format("{0}:{1}:{2}:{3}", uuid, secretWord, dateStr, login);
            //Структура: MD5(UUID: секретное слово: дата:login), где
            //«:» -разделитель.
            //«секретное слово» -статичное неизменяемое, жестко прошитое в АРМ Оператора, длиной не менее 12 символов, наличие прописных и строчных букв, десятичных цифр, неалфавитных символов.Хранится как строка в 16 - ном представлении.
            //«дата»  -дата в формате даты MMDDYYYYHH, где MM – месяц, DD – день, YYYY – год, HH – час.
            //"login" - имя пользователя, под которым зарегистрирован оператор.

            return MD5Hash(str);
        }

        /// <summary>
        /// вычисление MD5
        /// </summary>
        public static readonly Func<string, string> MD5Hash = (input) =>
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        };



     
    }
}
