using Newtonsoft.Json;
using System;
using System.Reflection;

namespace P3tr0viCh.Utils
{
    public static class PasswordProperty
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        public class AssemblyPasswordAttribute : Attribute
        {
            public string Value { get; }

            public AssemblyPasswordAttribute(string value) => Value = value;
        }

        public class PasswordConverter : JsonConverter<string>
        {
            public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var decryptedValue = string.Empty;

                try
                {
                    var key = new Misc.AssemblyDecorator().Assembly.GetCustomAttribute<AssemblyPasswordAttribute>()?.Value;

                    decryptedValue = Crypto.Decrypt((string)reader.Value, key);
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
                    var key = new Misc.AssemblyDecorator().Assembly.GetCustomAttribute<AssemblyPasswordAttribute>()?.Value;

                    encryptedValue = Crypto.Encrypt(value, key);
                }
                catch (Exception e)
                {
                    DebugWrite.Error(e);
                }

                writer.WriteValue(encryptedValue);
            }
        }
    }
}