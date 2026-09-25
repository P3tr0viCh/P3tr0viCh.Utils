using Newtonsoft.Json;
using P3tr0viCh.Utils.Attributes;
using System;
using System.Reflection;

namespace P3tr0viCh.Utils.Converters
{
    public class PasswordConverter : JsonConverter<string>
    {
        private readonly string securityKey;

        public PasswordConverter()
        {
            securityKey = new AssemblyDecorator().Assembly.GetCustomAttribute<AssemblySecurityKeyAttribute>()?.Value;
        }

        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var decryptedValue = string.Empty;

            try
            {
                decryptedValue = Crypto.Decrypt((string)reader.Value, securityKey);
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
                encryptedValue = Crypto.Encrypt(value, securityKey);
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);
            }

            writer.WriteValue(encryptedValue);
        }
    }
}