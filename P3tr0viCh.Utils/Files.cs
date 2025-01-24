﻿using System;
using System.IO;
using System.Windows.Forms;
using static P3tr0viCh.Utils.Exceptions;

namespace P3tr0viCh.Utils
{
    public static class Files
    {
        public const string ExtConfig = "config";
        public const string ExtSqLite = "sqlite";

        public static string ExecutableName()
        {
            return Path.GetFileNameWithoutExtension(Application.ExecutablePath);
        }

        public static string ExecutableDirectory()
        {
            return Path.GetDirectoryName(Application.ExecutablePath);
        }

        public static string AppDataLocalDirectory(string dir)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), dir);
        }

        public static string AppDataLocalDirectory()
        {
            return AppDataLocalDirectory(ExecutableName());
        }

        public static string AppDataRoamingDirectory(string dir)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), dir);
        }

        public static string AppDataRoamingDirectory()
        {
            return AppDataRoamingDirectory(ExecutableName());
        }

        public static string AppDataCommonDirectory(string dir)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), dir);
        }

        public static string AppDataCommonDirectory()
        {
            return AppDataCommonDirectory(ExecutableName());
        }

        public static string SettingsFileName(string fileName)
        {
            return Path.ChangeExtension(Path.GetFileName(fileName), ExtConfig);
        }

        public static string SettingsFileName()
        {
            return SettingsFileName(Application.ExecutablePath);
        }

        public static string DatabaseFileName(string fileName)
        {
            return Path.ChangeExtension(Path.GetFileName(fileName), ExtSqLite);
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
            if (fileName.IsEmpty()) fileName = "xxx";

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

        public static string PathNormalize(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToUpperInvariant();
        }

        public static bool PathEquals(string path1, string path2)
        {
            return string.Equals(PathNormalize(path1), PathNormalize(path2));
        }

        public static void CheckDirectoryExists(string path)
        {
            if (path.IsEmpty() || !Directory.Exists(path))
            {
                throw new DirectoryNotExistsException(path);
            }
        }

        public static void CheckFileExists(string path)
        {
            if (path.IsEmpty() || !File.Exists(path))
            {
                throw new FileNotExistsException(path);
            }
        }
    }
}