using System.Security.Cryptography;
using System.Text;

namespace UtNhanDrug_BE.Hepper.HashingAlgorithms
{
    public class HashingAlgorithmPassword
    {
        public static string PasswordHashMD5(string password)
        {
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder passwordEncode = new StringBuilder();
            foreach(byte b in hash)
            {
                passwordEncode.Append(b.ToString("x2"));
            }
            return passwordEncode.ToString();
        }
        
        public static string PasswordHashSHA1(string password)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder passwordEncode = new StringBuilder();
            foreach(byte b in hash)
            {
                passwordEncode.Append(b.ToString("x2"));
            }
            return passwordEncode.ToString();
        }
        
        public static string PasswordHashSHA512(string password)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder passwordEncode = new StringBuilder();
            foreach(byte b in hash)
            {
                passwordEncode.Append(b.ToString("x2"));
            }
            return passwordEncode.ToString();
        }



    }
}
