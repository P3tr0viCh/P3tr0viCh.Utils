using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace P3tr0viCh.Utils
{
    public static class ColorConverterExtensions
    {
        public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

        public static string ToRgbString(this Color c) => $"RGB({c.R}, {c.G}, {c.B})";
    }

    public static class TimeSpanConverterExtensions
    {
        public static string ToHoursMinutesString(this TimeSpan timeSpan) =>
            $"{((timeSpan.TotalHours > 23) ? (int)timeSpan.TotalHours : timeSpan.Hours):D2}:{timeSpan.Minutes:D2}";
    }

    public static class ControlExtentions
    {
        public static void InvokeIfNeeded(this Control control, Action doit)
        {
            if (control.InvokeRequired)
                control.Invoke(doit);
            else
                doit();
        }

        public static void InvokeIfNeeded<T>(this Control control, Action<T> doit, T arg)
        {
            if (control.InvokeRequired)
                control.Invoke(doit, arg);
            else
                doit(arg);
        }

        public static bool MouseIsOverControl(this Control control) =>
            control.ClientRectangle.Contains(control.PointToClient(Cursor.Position));
    }

    public static class EnumExtensions
    {
        public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue);
    }

    public static class BitArrayExtensions
    {
        public static byte ToByte(this BitArray bitArray)
        {
            byte result = 0;
            for (byte index = 0, m = 1; index < 8; index++, m *= 2)
                result += bitArray.Get(index) ? m : (byte)0;
            return result;
        }
    }

    public static class DateTimeExtensions
    {
        public static DateTime ChangeTime(this DateTime dateTime, int hour, int minute, int second, int millisecond)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                hour, minute, second, millisecond,
                dateTime.Kind);
        }
    }

    public static class TimerExtensions
    {
        public static void Restart(this Timer timer)
        {
            timer.Stop();
            timer.Start();
        }
    }

    public static class DataGridViewExtensions
    {
        public static bool ColumnExists(this DataGridView dataGridView, int index)
        {
            return dataGridView.Columns[index] != null;
        }

        public static bool ColumnExists(this DataGridView dataGridView, string columnName)
        {
            return dataGridView.Columns[columnName] != null;
        }
    }

    public static class RandomExtensions
    {
        public static bool NextBool(this Random random) => random.Next(2) == 0;
    }

    public static class ContextMenuStripExtensions
    {
        public static void ShowFromButton(this ContextMenuStrip contextMenuStrip, Button button)
        {
            contextMenuStrip.Show(button.PointToScreen(new Point(0, button.Height)));
        }
    }
}