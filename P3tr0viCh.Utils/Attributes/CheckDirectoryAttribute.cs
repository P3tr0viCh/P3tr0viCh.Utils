using System;

namespace P3tr0viCh.Utils.Attributes
{
    public class CheckDirectoryAttribute : Attribute
    {
        public bool CanEmpty { get; } = true;

        public bool CheckExists { get; } = true;

        public bool SetFullPath { get; } = true;

        public CheckDirectoryAttribute() { }

        public CheckDirectoryAttribute(bool checkExists)
        {
            CheckExists = checkExists;
        }

        public CheckDirectoryAttribute(bool canEmpty, bool checkExists, bool setFullPath)
        {
            CanEmpty = canEmpty;
            CheckExists = checkExists;
            SetFullPath = setFullPath;
        }
    }
}