using System;
using System.Linq;

namespace UtNhanDrug_BE.Hepper
{
    public class KeyGenerator
    {
        private static readonly Random random = new Random();
        public static string GetUniqueKey(int size)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, size)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
