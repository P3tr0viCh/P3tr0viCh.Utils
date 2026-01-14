using System;

namespace P3tr0viCh.Utils.Attributes
{
    public class CheckDirectoryAttribute : Attribute
    {
        public bool CanEmpty { get; set; } = true;

        public bool CheckExists { get; set; } = true;

        public bool SetFullPath { get; set; } = true;

        public CheckDirectoryAttribute() { }
    }
}