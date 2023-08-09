using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace P3tr0viCh.Utils
{
    public static class License
    {
        public class LicenseInfo<T>
        {
            public string Id { get; set; } = string.Empty;

            public string Name { get; set; } = string.Empty;

            public T Data { get; set; } = default;

            public void Clear()
            {
                Id = string.Empty;
                Name = string.Empty;
                Data = default;
            }
        }

        public enum LoadResult
        {
            Success,
            FileNotExists,
            ErrorCrc,
            ErrorReading,
            ErrorDecrypt,
            ErrorDeserialize,
            EmptySecurityKey,
        }

        private class InternalLicenseInfo<T>
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string Crc { get; set; }

            public string Data { get; set; }

            public InternalLicenseInfo()
            {
            }

            public InternalLicenseInfo(LicenseInfo<T> licenseInfo)
            {
                FromLicenseInfo(licenseInfo);

                Crc = GetCrc();
            }

            public string GetCrc()
            {
                return Crypto.Crc(Id + Name + Data);
            }

            public void FromLicenseInfo(LicenseInfo<T> licenseInfo)
            {
                Id = licenseInfo.Id;
                Name = licenseInfo.Name;
                Data = JsonConvert.SerializeObject(licenseInfo.Data);
            }

            public void ToLicenseInfo(LicenseInfo<T> licenseInfo)
            {
                licenseInfo.Id = Id;
                licenseInfo.Name = Name;
                licenseInfo.Data = JsonConvert.DeserializeObject<T>(Data);
            }
        }

        public class LicenseFile<T>
        {
            private string fileName = string.Empty;
            public string FileName
            {
                get
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        fileName = Path.Combine(Files.ExecutableDirectory(), Files.ExecutableName() + ".key");
                    }

                    return fileName;
                }
                set
                {
                    fileName = value;
                }
            }

            public string SecurityKey { get; set; } = string.Empty;

            public LicenseInfo<T> LicenseInfo { get; } = new LicenseInfo<T>();

            public void Save()
            {
                using (var writer = File.CreateText(FileName))
                {
                    var internalLicenseInfo = new InternalLicenseInfo<T>(LicenseInfo);

                    internalLicenseInfo.Crc = Crypto.Encrypt(internalLicenseInfo.Crc, SecurityKey);
                    internalLicenseInfo.Data = Crypto.Encrypt(internalLicenseInfo.Data, SecurityKey);

                    var content = JsonConvert.SerializeObject(internalLicenseInfo, Formatting.Indented);

                    writer.Write(content);
                }
            }

            public LoadResult Load()
            {
                LicenseInfo.Clear();

                if (string.IsNullOrEmpty(SecurityKey)) return LoadResult.EmptySecurityKey;

                if (!File.Exists(FileName)) return LoadResult.FileNotExists;

                try
                {
                    using (var reader = File.OpenText(FileName))
                    {
                        var internalLicenseInfo = JsonConvert.DeserializeObject<InternalLicenseInfo<T>>(reader.ReadToEnd());

                        try
                        {
                            internalLicenseInfo.Crc = Crypto.Decrypt(internalLicenseInfo.Crc, SecurityKey);
                            internalLicenseInfo.Data = Crypto.Decrypt(internalLicenseInfo.Data, SecurityKey);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);

                            return LoadResult.ErrorDecrypt;
                        }

                        var crc = internalLicenseInfo.GetCrc();

                        if (!string.Equals(crc, internalLicenseInfo.Crc))
                        {
                            return LoadResult.ErrorCrc;
                        }

                        internalLicenseInfo.ToLicenseInfo(LicenseInfo);
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);

                    if (e is JsonException)
                    {
                        return LoadResult.ErrorDeserialize;
                    }

                    return LoadResult.ErrorReading;
                }

                return LoadResult.Success;
            }
        }
    }
}