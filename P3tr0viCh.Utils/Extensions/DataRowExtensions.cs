using System;
using System.Data;

namespace P3tr0viCh.Utils.Extensions
{
    public static class DataRowExtensions
    {
        public static string AsString(this DataRow row, string columnName) => Convert.ToString(row[columnName]);
        
        public static string AsStringNullable(this DataRow row, string columnName) {
            if (row[columnName] == null) return null;

            var s = row.AsString(columnName);

            if (s.IsEmpty()) return null;
            
            return s;
        }

        public static DateTime AsDateTime(this DataRow row, string columnName) => Convert.ToDateTime(row[columnName]);

        public static DateTime? AsDateTimeNullable(this DataRow row, string columnName) {
            if (row[columnName] == null) return null;

            var dt = row.AsDateTime(columnName);

            if (dt == default) return null;

            return dt;
        }
        
        public static double AsDouble(this DataRow row, string columnName) => Convert.ToDouble(row[columnName]);

        public static double? AsDoubleNullable(this DataRow row, string columnName)
        {
            if (row[columnName] == null) return null;

            var d = row.AsDouble(columnName);

            if (d == default) return null;

            return d;
        }
    }
}