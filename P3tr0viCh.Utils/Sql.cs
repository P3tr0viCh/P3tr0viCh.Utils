using System;
using System.Linq;

namespace P3tr0viCh.Utils
{
    public static class Sql
    {
        public static long NewId = 0;

        public class Query
        {
            public string fields = string.Empty;
            public string table = string.Empty;
            public string where = string.Empty;
            public string group = string.Empty;
            public string order = string.Empty;
            public int limit = 0;
            public int offset = 0;

            public string Select()
            {
                if (string.IsNullOrEmpty(fields))
                {
                    fields = "*";
                }

                var sql = "SELECT " + fields;

                sql = sql.JoinExcludeEmpty(" FROM ", table);
                sql = sql.JoinExcludeEmpty(" WHERE ", where);
                sql = sql.JoinExcludeEmpty(" GROUP BY ", group);
                sql = sql.JoinExcludeEmpty(" ORDER BY ", order);

                if (limit > 0)
                {
                    sql = sql.JoinExcludeEmpty(" LIMIT ", limit.ToString());
                }

                if (offset > 0)
                {
                    sql = sql.JoinExcludeEmpty(" OFFSET ", offset.ToString());
                }

                return sql;
            }
        }

        public static string TableName<T>()
        {
            dynamic tableattr = typeof(T).GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == "TableAttribute");

            if (tableattr != null) return tableattr.Name;

            return string.Empty;
        }

        public static string FieldName(string name, string table = null, string alias = null)
        {
            var sql = name.ToLower();

            if (!string.IsNullOrEmpty(table)) sql = table + "." + sql;

            if (!string.IsNullOrEmpty(alias)) sql += " AS " + alias;

            return sql;
        }
    }
}