using P3tr0viCh.Utils.Properties;
using System.IO;

namespace P3tr0viCh.Utils
{
    public static class Exceptions
    {
        public class FileNotExistsException : FileNotFoundException
        {
            public FileNotExistsException() : base() { }

            public FileNotExistsException(string fileName) : base(Resources.ExceptionFileNotExists, fileName) { }
        }

        public class DirectoryNotExistsException : DirectoryNotFoundException
        {
            private readonly string path;

            public string Path => path;

            public DirectoryNotExistsException() : base() { }

            public DirectoryNotExistsException(string path) : base(string.Format(Resources.ExceptionDirectoryNotExists, path))
            {
                this.path = path;
            }
        }
    }
}