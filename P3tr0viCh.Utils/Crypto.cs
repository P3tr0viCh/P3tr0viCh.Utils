using System.Security.Cryptography;
using System.Text;
using System;

namespace P3tr0viCh.Utils
{
    public class Crypto
    {
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

        public static string Crc(string value)
        {
            if (value.IsEmpty()) return string.Empty;

            using (var MD5CryptoService = new MD5CryptoServiceProvider())
            {
                var hash = MD5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(value));

                return BitConverter.ToString(hash);
            }
        }
    }
}