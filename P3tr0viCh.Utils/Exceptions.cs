using P3tr0viCh.Utils.Extensions;
using P3tr0viCh.Utils.Properties;
using System;
using System.IO;
using System.Net;

namespace P3tr0viCh.Utils.Exceptions
{
    public class FileNotExistsException : FileNotFoundException
    {
        public FileNotExistsException() : base() { }

        public FileNotExistsException(string message, string fileName) :
            base(string.Format(message, fileName), fileName)
        { }

        public FileNotExistsException(string fileName) :
            this(ResourcesExceptions.FileNotExists, fileName)
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
            this(ResourcesExceptions.DirectoryNotExists, path)
        { }
    }

    public class FileBadFormatException : FileNotFoundException
    {
        public FileBadFormatException() : base(ResourcesExceptions.FileBadFormat) { }
        
        public FileBadFormatException(string message, string fileName) :
            base(string.Format(message, fileName), fileName)
        { }
        
        public FileBadFormatException(string fileName) :
            this(ResourcesExceptions.FileBadFormatWithFileName, fileName)
        { }
    }

    public class FileZeroLengthException : FileBadFormatException
    {
        public FileZeroLengthException() : base(ResourcesExceptions.FileZeroLength) { }
        
        public FileZeroLengthException(string fileName) :
            base(ResourcesExceptions.FileZeroLengthWithFileName, fileName)
        { }
    }

    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCode StatusCode { get; } = HttpStatusCode.OK;

        public HttpStatusCodeException() : base() { }

        public HttpStatusCodeException(HttpStatusCode statusCode) : base($"{statusCode.ToInt()}: {statusCode}")
        {
            StatusCode = statusCode;
        }
    }

    public class PropertyException : Exception
    {
        public string PropertyName { get; } = string.Empty;

        public PropertyException() : base() { }

        public PropertyException(Exception e) : base(string.Empty, e)
        {
        }

        public PropertyException(string propertyName) :
            base(string.Format(ResourcesExceptions.Property, propertyName))
        {
            PropertyName = propertyName;
        }

        public PropertyException(string propertyName, Exception e) :
            base(string.Format(ResourcesExceptions.PropertyWithException, propertyName, e.Message))
        {
            PropertyName = propertyName;
        }
    }

    public class WrongHashException : Exception
    {
    }
}