﻿using Newtonsoft.Json;
using System;
using System.Reflection;

namespace P3tr0viCh.Utils
{
    public static class PasswordProperty
    {
        [AttributeUsage(AttributeTargets.Assembly)]
        public class AssemblySecurityKeyAttribute : Attribute
        {
            public string Value { get; }

            public AssemblySecurityKeyAttribute(string value) => Value = value;
        }

        public static string GetSecurityKeyAttribute()
        {
            return new AssemblyDecorator().Assembly.GetCustomAttribute<AssemblySecurityKeyAttribute>()?.Value;
        }

        public class PasswordConverter : JsonConverter<string>
        {
            public override string ReadJson(JsonReader reader, Type objectType, string existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var decryptedValue = string.Empty;

                try
                {
                    var key = GetSecurityKeyAttribute();

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
                    var key = GetSecurityKeyAttribute();

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