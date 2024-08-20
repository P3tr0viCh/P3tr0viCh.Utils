using P3tr0viCh.Utils.Properties;
using System;
using System.ComponentModel;

namespace P3tr0viCh.Utils
{
    [TypeConverter(typeof(PropertySortedConverter))]
    public class DBConnection
    {
        public enum DBType
        {
            None = 0,
            MySql,
        }

        public DBConnection()
        {
        }

        public DBConnection(DBType type) => Type = type;

        [PropertyOrder(100)]
        public DBType Type { get; set; } = DBType.None;

        [PropertyOrder(110)]
        public string Host { get; set; }
        [PropertyOrder(111)]
        public int Port { get; set; } = 0;

        [PropertyOrder(120)]
        public string Database { get; set; }

        [PropertyOrder(130)]
        public Login Login { get; set; } = new Login();

        [PropertyOrder(140)]
        public string Driver { get; set; }

        private string ConnectionStringMySql()
        {
            return string.Format(Resources.ConnectionStringMySql,
                Host.IsEmpty() ? "localhost" : Host,
                Port == 0 ? 3306 : Port,
                Database,
                Login?.User, Login?.Password,
                Driver);
        }

        public string ConnectionString()
        {
            switch (Type)
            {
                case DBType.MySql:
                    return ConnectionStringMySql();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}