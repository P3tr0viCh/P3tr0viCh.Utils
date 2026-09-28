using P3tr0viCh.Utils.Attributes;
using P3tr0viCh.Utils.Extensions;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace P3tr0viCh.Utils
{
    public class Crypto
    {
        private static readonly Lazy<string> lazySecurityKey = new Lazy<string>(() => GetSecurityKey());

        public static string SecurityKey => lazySecurityKey.Value;
        
        private static string GetSecurityKey() => 
            new AssemblyDecorator().Assembly.GetCustomAttribute<AssemblySecurityKeyAttribute>()?.Value;

        private static byte[] SecurityKeyToArray(string securityKey)
        {
            using (var MD5CryptoService = new MD5CryptoServiceProvider())
            {
                return MD5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(securityKey));
            }
        }

        private static TripleDESCryptoServiceProvider GetTripleDESCryptoService(string securityKey)
        {
            return new TripleDESCryptoServiceProvider
            {
                Key = SecurityKeyToArray(securityKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
        }

        public static string Encrypt(string plainText, string securityKey)
        {
            if (plainText.IsEmpty() || securityKey.IsEmpty()) return string.Empty;

            using (var TripleDESCryptoService = GetTripleDESCryptoService(securityKey))
            using (var CryptoTransform = TripleDESCryptoService.CreateEncryptor())
            {
                var encryptedArray = Encoding.UTF8.GetBytes(plainText);

                var resultArray = CryptoTransform.TransformFinalBlock(encryptedArray, 0, encryptedArray.Length);

                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
        }

        public static string Decrypt(string cipherText, string securityKey)
        {
            if (cipherText.IsEmpty() || securityKey.IsEmpty()) return string.Empty;

            using (var TripleDESCryptoService = GetTripleDESCryptoService(securityKey))
            using (var CryptoTransform = TripleDESCryptoService.CreateDecryptor())
            {
                var encryptArray = Convert.FromBase64String(cipherText);

                var resultArray = CryptoTransform.TransformFinalBlock(encryptArray, 0, encryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
        }

        public static string MD5Hash(string value)
        {
            if (value.IsEmpty()) return string.Empty;

            using (var MD5CryptoService = new MD5CryptoServiceProvider())
            {
                var buffer = Encoding.UTF8.GetBytes(value);

                var hash = MD5CryptoService.ComputeHash(buffer);

                return BitConverter.ToString(hash);
            }
        }

        public static string HMACSHA256Hash(string value, string securityKey)
        {
            if (value.IsEmpty() || securityKey.IsEmpty()) return string.Empty;

            var key = Encoding.UTF8.GetBytes(securityKey);

            using (var hmac = new HMACSHA256(key))
            {
                var buffer = Encoding.UTF8.GetBytes(value);

                var hash = hmac.ComputeHash(buffer);

                return Convert.ToBase64String(hash);
            }
        }
    }
}