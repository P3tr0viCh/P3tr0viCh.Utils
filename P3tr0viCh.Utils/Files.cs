using System;
using System.IO;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class Files
    {
        public static string SettingsDirectory()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.GetFileNameWithoutExtension(Application.ExecutablePath));
        }

        public static string SettingsFileName()
        {
            return Path.GetFileNameWithoutExtension(Application.ExecutablePath) +
#if DEBUG
                ".debug" +
#endif
                ".config";
        }

        public static string DatabaseFileName()
        {
            return Path.Combine(SettingsDirectory(),
                Path.GetFileNameWithoutExtension(Application.ExecutablePath) +
#if DEBUG
                ".debug" +
#endif
                ".sqlite");
        }

        public static string TempFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) fileName = "xxx";

            var dir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Application.ExecutablePath));

            Directory.CreateDirectory(dir);

            return Path.Combine(dir, fileName);
        }
    }
}