using System;
using System.Drawing;

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
}