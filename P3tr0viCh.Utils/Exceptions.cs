using System.IO;

namespace P3tr0viCh.Utils
{
    public static class Exceptions
    {
        public class FilePathEmptyException : FileNotFoundException
        {
            public FilePathEmptyException() : base("") { }
        }

        public class DirectoryPathEmptyException : DirectoryNotFoundException
        {
            public DirectoryPathEmptyException() : base("") { }
        }
    }
}