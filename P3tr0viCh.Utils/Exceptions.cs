using P3tr0viCh.Utils.Properties;
using System;
using System.IO;
using System.Net;

namespace P3tr0viCh.Utils
{
    public static class Exceptions
    {
        public class FileNotExistsException : FileNotFoundException
        {
            public FileNotExistsException() : base() { }

            public FileNotExistsException(string message, string fileName) :
                base(string.Format(message, fileName), fileName)
            { }

            public FileNotExistsException(string fileName) :
                this(Resources.ExceptionFileNotExists, fileName)
            { }
        }

        public class DirectoryNotExistsException : DirectoryNotFoundException
        {
            public string Path { get; }

            public DirectoryNotExistsException() : base() { }

            public DirectoryNotExistsException(string message, string path) :
                base(string.Format(message, path))
            {
                Path = path;
            }

            public DirectoryNotExistsException(string path) :
                this(Resources.ExceptionDirectoryNotExists, path)
            { }
        }

        public class FileBadFormatException : FileNotFoundException
        {
            public FileBadFormatException() : base(Resources.ExceptionFileBadFormat) { }
            public FileBadFormatException(string message) : base(message) { }
            public FileBadFormatException(string message, string fileName) : base(message, fileName) { }
        }

        public class FileZeroLengthException : FileBadFormatException
        {
            public FileZeroLengthException() : base(Resources.ExceptionFileZeroLength) { }
            public FileZeroLengthException(string message) : base(message) { }
            public FileZeroLengthException(string message, string fileName) : base(message, fileName) { }
        }

        public class HttpStatusCodeException : Exception
        {
            public HttpStatusCodeException() : base() { }

            public HttpStatusCode StatusCode { get; } = HttpStatusCode.OK;

            public HttpStatusCodeException(HttpStatusCode statusCode) : base($"{statusCode.ToInt()}: {statusCode}")
            {
                StatusCode = statusCode;
            }
        }
    }
}