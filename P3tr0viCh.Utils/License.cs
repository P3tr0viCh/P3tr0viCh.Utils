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

        private class InternalLicenseInfoData
        {
            public string Crc { get; set; }

            public string Data { get; set; }
        }

        private class InternalLicenseInfo<T>
        {
            public string Id { get; set; }

            public string Name { get; set; }

            [JsonIgnore]
            public InternalLicenseInfoData InternalData { get; set; } = new InternalLicenseInfoData();

            public string Data { get; set; }

            public InternalLicenseInfo()
            {
            }

            public InternalLicenseInfo(LicenseInfo<T> licenseInfo)
            {
                FromLicenseInfo(licenseInfo);
            }

            public string GetCrc()
            {
                return Crypto.Crc(Id + Name + InternalData.Data);
            }

            public void FromLicenseInfo(LicenseInfo<T> licenseInfo)
            {
                Id = licenseInfo.Id;
                Name = licenseInfo.Name;

                InternalData.Data = JsonConvert.SerializeObject(licenseInfo.Data);
                
                InternalData.Crc = GetCrc();

                Data = JsonConvert.SerializeObject(InternalData);
            }

            public void ToLicenseInfo(LicenseInfo<T> licenseInfo)
            {
                licenseInfo.Id = Id;
                licenseInfo.Name = Name;
                licenseInfo.Data = JsonConvert.DeserializeObject<T>(InternalData.Data);
            }
        }

        public class LicenseFile<T>
        {
            private string fileName = string.Empty;
            public string FileName
            {
                get
                {
                    if (fileName.IsEmpty())
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

                    internalLicenseInfo.Data = Crypto.Encrypt(internalLicenseInfo.Data, SecurityKey);

                    var content = JsonConvert.SerializeObject(internalLicenseInfo, Formatting.Indented);

                    writer.Write(content);
                }
            }

            public LoadResult Load()
            {
                LicenseInfo.Clear();

                if (SecurityKey.IsEmpty()) return LoadResult.EmptySecurityKey;

                if (!File.Exists(FileName)) return LoadResult.FileNotExists;

                try
                {
                    using (var reader = File.OpenText(FileName))
                    {
                        var internalLicenseInfo = JsonConvert.DeserializeObject<InternalLicenseInfo<T>>(reader.ReadToEnd());

                        try
                        {
                            internalLicenseInfo.Data = Crypto.Decrypt(internalLicenseInfo.Data, SecurityKey);
                        }
                        catch (Exception e)
                        {
                            DebugWrite.Line(e.Message);

                            return LoadResult.ErrorDecrypt;
                        }

                        internalLicenseInfo.InternalData = JsonConvert.DeserializeObject<InternalLicenseInfoData>(internalLicenseInfo.Data);

                        var crc = internalLicenseInfo.GetCrc();

                        if (!string.Equals(crc, internalLicenseInfo.InternalData.Crc))
                        {
                            return LoadResult.ErrorCrc;
                        }

                        internalLicenseInfo.ToLicenseInfo(LicenseInfo);
                    }
                }
                catch (Exception e)
                {
                    DebugWrite.Line(e.Message);

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