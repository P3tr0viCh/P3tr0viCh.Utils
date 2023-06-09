﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public class SettingsBase<T> where T : new()
    {
        public class FormState
        {
            public Rectangle Bounds { get; set; } = default;
            public bool Maximized { get; set; } = false;
        }

        public class ColumnsState
        {
            public int[] Widths { get; set; } = null;
        }

        private static T defaultInstance = new T();

        public static T Default
        {
            get
            {
                return defaultInstance;
            }
        }

        private static string directory = string.Empty;
        public static string Directory
        {
            get
            {
                return directory;
            }
            set
            {
                directory = value;
                filePath = string.Empty;
            }
        }

        private static string fileName = string.Empty;
        public static string FileName
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

        private static string filePath = string.Empty;
        public static string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    if (string.IsNullOrEmpty(Directory))
                    {
                        directory = Files.AppDataDirectory();
                    }

                    if (string.IsNullOrEmpty(FileName))
                    {
                        fileName = Files.SettingsFileName();
                    }

                    filePath = Path.Combine(Directory, FileName);
                }

                return filePath;
            }
        }

        public static FormState SaveFormState(Form form)
        {
            var state = new FormState();

            switch (form.FormBorderStyle)
            {
                case FormBorderStyle.None:
                case FormBorderStyle.Sizable:
                    if (form.WindowState == FormWindowState.Maximized)
                    {
                        state.Maximized = true;
                    }
                    else
                    {
                        state.Maximized = false;

                        state.Bounds = form.Bounds;
                    }

                    break;
                case FormBorderStyle.Fixed3D:
                case FormBorderStyle.FixedSingle:
                case FormBorderStyle.FixedDialog:
                case FormBorderStyle.FixedToolWindow:
                    state.Bounds = new Rectangle(form.Left, form.Top, 0, 0);

                    break;
                case FormBorderStyle.SizableToolWindow:
                    state.Bounds = form.Bounds;

                    break;
                default:
                    break;
            }

            return state;
        }

        public static void LoadFormState(Form form, FormState state)
        {
            try
            {
                if (state == null)
                {
                    state = new FormState();
                }

                if (state.Bounds == default)
                {
                    state.Bounds = new Rectangle(
                        (Screen.FromControl(form).WorkingArea.Width - form.Width) / 2,
                        (Screen.FromControl(form).WorkingArea.Height - form.Height) / 2,
                        form.Width, form.Height);
                }

                form.StartPosition = FormStartPosition.Manual;

                switch (form.FormBorderStyle)
                {
                    case FormBorderStyle.None:
                    case FormBorderStyle.Sizable:
                        form.Bounds = state.Bounds;

                        if (state.Maximized)
                        {
                            form.WindowState = FormWindowState.Maximized;
                        }

                        break;
                    case FormBorderStyle.Fixed3D:
                    case FormBorderStyle.FixedSingle:
                    case FormBorderStyle.FixedDialog:
                    case FormBorderStyle.FixedToolWindow:
                        form.Location = new Point(state.Bounds.Left, state.Bounds.Top);

                        break;
                    case FormBorderStyle.SizableToolWindow:
                        form.Bounds = state.Bounds;

                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
        }

        public static ColumnsState SaveDataGridColumns(DataGridView dataGridView)
        {
            var columns = new ColumnsState();

            if (columns.Widths == null)
            {
                columns.Widths = new int[dataGridView.Columns.Count];
            }

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                columns.Widths[column.Index] = column.Width;
            }

            return columns;
        }

        public static void LoadDataGridColumns(DataGridView dataGridView, ColumnsState columns)
        {
            try
            {
                if (columns == null) return;

                if (columns.Widths == null) return;

                if (columns.Widths.Length < dataGridView.Columns.Count) return;

                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.Width = columns.Widths[column.Index];
                }
            }
            catch (Exception)
            {
            }
        }

        public void Save()
        {
            try
            {
                if (!System.IO.Directory.Exists(Directory))
                {
                    System.IO.Directory.CreateDirectory(Directory);
                }

                using (var writer = File.CreateText(FilePath))
                {
                    var content = JsonConvert.SerializeObject(defaultInstance, Formatting.Indented);

                    writer.Write(content);
                }
            }
            catch
            {
                Debug.WriteLine("appsettings save");
            }
        }

        public void Load()
        {
            if (!File.Exists(FilePath)) return;

            try
            {
                using (var reader = File.OpenText(FilePath))
                {
                    defaultInstance = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                }
            }
            catch
            {
                Debug.WriteLine("appsettings load");
            }
        }
    }
}