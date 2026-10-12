using Newtonsoft.Json;
using P3tr0viCh.Utils.Exceptions;
using P3tr0viCh.Utils.Extensions;
using System;
using System.IO;

namespace P3tr0viCh.Utils.Storage
{
    public class ObjectStorage<T> : IObjectStorage where T : class, IObjectPersistence, new()
    {
        private T data = new T();

        private string directory = DefaultDirectory;

        private string fileName = DefaultFileName;

        private string filePath = string.Empty;

        private string filePathHash = string.Empty;

        public T Data => data;

        public IObjectPersistence Object => data;

        public static string DefaultDirectory => Files.AppDataLocalDirectory();
        public static string DefaultFileName => Files.SettingsFileName();

        public string Directory
        {
            get => directory;
            set
            {
                directory = value.IsEmpty() ? DefaultDirectory : value;

                UpdateFilePath();
            }
        }

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value.IsEmpty() ? DefaultFileName : value;

                UpdateFilePath();
            }
        }

        public string FilePath => filePath;

        public string FilePathHash => filePathHash;

        public bool UseHash { get; set; } = false;

        public Exception LastError { get; private set; } = null;

        public bool HasError => LastError != null;

        private void UpdateFilePath()
        {
            filePath = Path.Combine(Directory, FileName);

            filePathHash = filePath + "." + Files.ExtConfigHash;
        }

        private string GetHash(string value) => Crypto.HMACSHA256Hash(value, Crypto.SecurityKey);

        private void SaveObject(string content)
        {
            File.WriteAllText(FilePath, content);
        }

        private void SaveHash(string content)
        {
            if (UseHash)
            {
                var hash = GetHash(content);

                File.WriteAllText(FilePathHash, hash);
            }
            else
            {
                File.Delete(FilePathHash);
            }
        }

        public virtual bool Save()
        {
            LastError = null;

            try
            {
                System.IO.Directory.CreateDirectory(Directory);

                var content = JsonConvert.SerializeObject(data, Formatting.Indented);

                SaveHash(content);

                SaveObject(content);

                return true;
            }
            catch (Exception e)
            {
                LastError = e;

                return false;
            }
        }

        private string LoadObject()
        {
            Files.CheckFileExists(FilePath);

            if (Files.IsFileEmpty(FilePath)) throw new FileZeroLengthException(FilePath);

            var content = File.ReadAllText(FilePath);

            data = JsonConvert.DeserializeObject<T>(content);

            if (data is null) throw new NullReferenceException();

            data.Check();

            return content;
        }

        private void CheckHash(string content)
        {
            if (!UseHash) return;

            Files.CheckFileExists(FilePathHash);

            var hash = GetHash(content);

            var hashSaved = File.ReadAllText(FilePathHash);

            if (hashSaved.IsEmpty()) throw new FileZeroLengthException(FilePathHash);

            if (hashSaved != hash)
            {
                throw new WrongHashException();
            }
        }

        public virtual bool Load()
        {
            LastError = null;

            try
            {
                var content = LoadObject();

                CheckHash(content);

                return true;
            }
            catch (Exception e)
            {
                LastError = e;

                data = new T();

                return false;
            }
        }
    }
}