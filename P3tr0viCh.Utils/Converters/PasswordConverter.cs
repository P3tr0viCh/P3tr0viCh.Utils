using Newtonsoft.Json;
using P3tr0viCh.Utils.Attributes;
using P3tr0viCh.Utils.Extensions;
using System;
using System.Reflection;

namespace P3tr0viCh.Utils.Converters
{
    public class PasswordConverter : JsonConverter<string>
    {
        private static string securityKey;

        public static string SecurityKey
        {
            get
            {
                if (securityKey.IsEmpty())
                {
                    securityKey = new AssemblyDecorator().Assembly.GetCustomAttribute<AssemblySecurityKeyAttribute>()?.Value;
                }

                return securityKey;
            }
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var decryptedValue = string.Empty;

            try
            {
                decryptedValue = Crypto.Decrypt((string)reader.Value, SecurityKey);
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);
            }

            return decryptedValue;
        }

        public override void WriteJson(JsonWriter writer, string value, JsonSerializer serializer)
        {
            var encryptedValue = string.Empty;

            try
            {
                encryptedValue = Crypto.Encrypt(value, SecurityKey);
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);
            }

            writer.WriteValue(encryptedValue);
        }
    }
}