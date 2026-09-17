using P3tr0viCh.Utils.Extensions;
using P3tr0viCh.Utils.Properties;
using System;
using System.IO;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class Log
    {
        public long MaxSize { get; set; } = 1024 * 1024; // 1 Mb

        private string directory = string.Empty;
        public string Directory
        {
            get => directory;
            set
            {
                if (value.IsEmpty())
                {
                    value = Files.ExecutableDirectory();
                }

                directory = Path.Combine(value, ResourcesLog.SubDirectory);

                filePath = string.Empty;
            }
        }

        private string fileName = string.Empty;
        public string FileName
        {
            get => fileName;
            set
            {
                if (value.IsEmpty())
                {
                    value = Files.ExecutableName();
                }

                fileName = value;

                filePath = string.Empty;
            }
        }

        private string filePath = string.Empty;
        public string FilePath
        {
            get
            {
                if (filePath.IsEmpty())
                {
                    filePath = Path.Combine(Directory, FileName + "." + ResourcesLog.Ext);
                }

                return filePath;
            }
        }

        public Log()
        {
            Directory = string.Empty;
            FileName = string.Empty;
        }

        private void Append(string s)
        {
            try
            {
                var dateTime = DateTime.Now;

                if (!System.IO.Directory.Exists(Directory))
                {
                    System.IO.Directory.CreateDirectory(Directory);
                }

                if (MaxSize > 0 && File.Exists(FilePath))
                {
                    var fileSize = new FileInfo(FilePath).Length;

                    if (fileSize > MaxSize)
                    {
                        var backupFilePath = Path.Combine(Directory,
                            FileName + "_" + dateTime.ToString(ResourcesLog.FormatDateTimeFile) + "." + ResourcesLog.Ext);

                        File.Move(FilePath, backupFilePath);
                    }
                }

                File.AppendAllText(FilePath,
                    dateTime.ToString(ResourcesLog.FormatDateTimeText) +
                        Str.Space + s.Trim().SingleLine() + Environment.NewLine);
            }
            catch (Exception e)
            {
                DebugWrite.Line($"log write error: {e.Message}");
            }
        }

        [Obsolete()]
        public void Write(string s)
        {
            Append(s);
        }

        public void Info(string text)
        {
            Append(ResourcesLog.AppendInfo + Str.Space + text);
        }

        public void Info(string format, object arg0) => Info(string.Format(format, arg0));

        public void Info(string format, object arg0, object arg1) => Info(string.Format(format, arg0, arg1));

        public void Error(string text)
        {
            Append(ResourcesLog.AppendError + Str.Space + text);
        }

        public void Error(string format, object arg0) => Error(string.Format(format, arg0));

        public void Error(string format, object arg0, object arg1) => Error(string.Format(format, arg0, arg1));

        public void WriteProgramStart()
        {
            var assemblyDecorator = new AssemblyDecorator();

            Info(ResourcesLog.ProgramStart, Files.ExecutableName());

            Info(ResourcesLog.ProgramVersion, 
                assemblyDecorator.InformationalVersion, assemblyDecorator.BuildDate.ToString("yyyy-MM-dd"));

            Info(ResourcesLog.OSVersion, Environment.OSVersion.ToString());
            Info(ResourcesLog.MachineName, Environment.MachineName);
        }

        public void WriteProgramStop()
        {
            Info(ResourcesLog.ProgramStop);
        }

        public void WriteFormOpen(Form frm)
        {
            Info(ResourcesLog.FormOpen, frm.Name);
        }

        public void WriteFormClose(Form frm)
        {
            Info(ResourcesLog.FormClose, frm.Name, frm.DialogResult);
        }
    }
}