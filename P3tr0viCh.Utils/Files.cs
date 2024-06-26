﻿using System;
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

        public static string SettingsFileName(string fileName)
        {
            return Path.ChangeExtension(Path.GetFileName(fileName), "config");
        }

        public static string SettingsFileName()
        {
            return SettingsFileName(Application.ExecutablePath);
        }

        public static string DatabaseFileName(string fileName)
        {
            return Path.ChangeExtension(Path.GetFileName(fileName), "sqlite");
        }

        public static string DatabaseFileName()
        {
            return DatabaseFileName(Application.ExecutablePath);
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

        public static long FileLength(string fileName)
        {
            return new FileInfo(fileName).Length;
        }

        public static void DirectoryRename(string sourceDirFullName, string destDirOnlyName)
        {
            Directory.Move(sourceDirFullName, Path.Combine(Path.GetDirectoryName(sourceDirFullName), destDirOnlyName));
        }

        public static void DirectoryDelete(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}