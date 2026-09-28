using Newtonsoft.Json;
using System;

namespace P3tr0viCh.Utils.Converters
{
    public class PasswordConverter : JsonConverter<string>
    {
        public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var decryptedValue = string.Empty;

            try
            {
                decryptedValue = Crypto.Decrypt((string)reader.Value, Crypto.SecurityKey);
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
                encryptedValue = Crypto.Encrypt(value, Crypto.SecurityKey);
            }
            catch (Exception e)
            {
                DebugWrite.Error(e);
            }

            writer.WriteValue(encryptedValue);
        }
    }
}