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

            public FileNotExistsException(string fileName) : base(Resources.ExceptionFileNotExists, fileName) { }
        }

        public class DirectoryNotExistsException : DirectoryNotFoundException
        {
            public string Path { get; }

            public DirectoryNotExistsException() : base() { }

            public DirectoryNotExistsException(string path) : base(string.Format(Resources.ExceptionDirectoryNotExists, path))
            {
                Path = path;
            }
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