using System;
using System.IO;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class Files
    {
        public static string ExecutableName()
        {
            return Path.GetFileNameWithoutExtension(Application.ExecutablePath);
        }

        public static string ExecutableDirectory()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        public static string AppDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ExecutableName());
        }

        public static string SettingsFileName()
        {
            return ExecutableName() + ".config";
        }

        public static string DatabaseFileName()
        {
            return ExecutableName() + ".sqlite";
        }
        public static string TempDirectory()
        {
            var dir = Path.Combine(Path.GetTempPath(), ExecutableName());

            Directory.CreateDirectory(dir);

            return dir;
        }

        public static string TempFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = "xxx";

            return Path.Combine(TempDirectory(), fileName);
        }
    }
}