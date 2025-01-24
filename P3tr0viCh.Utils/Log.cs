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
            get
            {
                return directory;
            }
            set
            {
                directory = Path.Combine(value, Resources.LogSubDirectory);
                filePath = string.Empty;
            }
        }

        private string fileName = string.Empty;
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
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
                    if (Directory.IsEmpty())
                    {
                        directory = Files.ExecutableDirectory();
                    }

                    if (FileName.IsEmpty())
                    {
                        fileName = Files.ExecutableName();
                    }

                    filePath = Path.Combine(Directory, FileName + "." + Resources.LogExt);
                }

                return filePath;
            }
        }

        public void Write(string s)
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
                            FileName + "_" + dateTime.ToString(Resources.LogFormatDateTimeFile) + "." + Resources.LogExt);

                        File.Move(FilePath, backupFilePath);
                    }
                }

                File.AppendAllText(FilePath,
                    dateTime.ToString(Resources.LogFormatDateTimeText) +
                        Str.Space + s.Replace(Str.Eol, Str.Space).Trim() + Environment.NewLine);
            }
            catch (Exception e)
            {
                DebugWrite.Line($"log write error: {e.Message}");
            }
        }

        public void WriteProgramStart()
        {
            Write(string.Format(Resources.LogProgramStart, Files.ExecutableName(),
                new Misc.AssemblyDecorator().VersionString()));
        }

        public void WriteProgramStop()
        {
            Write(Resources.LogProgramStop);
        }

        public void WriteFormOpen(Form frm)
        {
            Write(string.Format(Resources.LogFormOpen, frm.Name));
        }

        public void WriteFormClose(Form frm)
        {
            Write(string.Format(Resources.LogFormClose, frm.Name, frm.DialogResult));
        }
    }
}