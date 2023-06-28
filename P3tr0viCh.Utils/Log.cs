using P3tr0viCh.Utils.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class Log
    {
        public long MaxSize { get; set; } = 1024 * 1024; // 1 Mb

        public string LogPath { get; set; } = Files.ExecutableDirectory();

        public string LogName { get; set; } = Files.ExecutableName();

        public void Write(string s)
        {
            try
            {
                var dateTime = DateTime.Now;

                var logPath = Path.Combine(LogPath, Resources.LogSubDirectory);

                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }

                var logFileName = Path.Combine(logPath, LogName + "." + Resources.LogExt);

                if (MaxSize > 0 && File.Exists(logFileName))
                {
                    var fileSize = new FileInfo(logFileName).Length;

                    if (fileSize > MaxSize)
                    {
                        var logFileNameOld = Path.Combine(logPath,
                            LogName + "_" + dateTime.ToString(Resources.LogFormatDateTimeFile) + "." + Resources.LogExt);

                        File.Move(logFileName, logFileNameOld);
                    }
                }

                File.AppendAllText(logFileName,
                    dateTime.ToString(Resources.LogFormatDateTimeText) +
                        Str.Space + s.Replace(Str.Eol, Str.Space).Trim() + Str.Eol);
            }
            catch (Exception e)
            {
                Debug.WriteLine("log write error: " + e.Message);
            }
        }

        public void WriteProgramStart()
        {
            Write(string.Format(Resources.LogProgramStart, Files.ExecutableName(), new Misc.AssemblyDecorator().ToString()));
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