using P3tr0viCh.Utils.Properties;
using System.ComponentModel;

namespace P3tr0viCh.Utils
{
    public class DataBaseConnection
    {
        public interface IConnection
        {
            string ConnectionString();
        }

        [TypeConverter(typeof(PropertySortedConverter))]
        [LocalizedAttribute.DisplayName("Connection.DisplayName", "Properties.Resources.Utils")]
        public abstract class Connection : IConnection
        {
            [PropertyOrder(300)]
            public Login Login { get; set; } = new Login();

            public abstract string ConnectionString();
        }

        public abstract class ConnectionServer : Connection
        {
            [PropertyOrder(100)]
            [LocalizedAttribute.DisplayName("Connection.Host.DisplayName", "Properties.Resources.Utils")]
            public string Host { get; set; }

            [PropertyOrder(101)]
            [LocalizedAttribute.DisplayName("Connection.Port.DisplayName", "Properties.Resources.Utils")]
            public int Port { get; set; } = 0;
        }

        public abstract class ConnectionServerDatabase : ConnectionServer
        {
            [PropertyOrder(200)]
            [LocalizedAttribute.DisplayName("Connection.Database.DisplayName", "Properties.Resources.Utils")]
            public string Database { get; set; }
        }

        public class ConnectionMySql : ConnectionServerDatabase
        {
            public const string DefaultHost = "localhost";
            public const int DefaultPort = 3306;

            public ConnectionMySql()
            {
                Host = DefaultHost;
                Port = DefaultPort;
            }

            public override string ConnectionString()
            {
                return string.Format(Resources.ConnectionStringMySql,
                    Host.IsEmpty() ? DefaultHost : Host,
                    Port == 0 ? DefaultPort : Port,
                    Database,
                    Login.User, Login.Password);
            }
        }

        public class ConnectionFireBird : ConnectionServerDatabase
        {
            public const string DefaultUser = "SYSDBA";
            public const string DefaultPassword = "masterkey";

            public const string DefaultHost = "localhost";
            public const int DefaultPort = 3050;

            public ConnectionFireBird()
            {
                Login.User = DefaultUser;
                Login.Password = DefaultPassword;

                Host = DefaultHost;
                Port = DefaultPort;
            }

            public override string ConnectionString()
            {
                return string.Format(Resources.ConnectionFireBird,
                    Host.IsEmpty() ? DefaultHost : Host,
                    Port == 0 ? DefaultPort : Port,
                    Database,
                    Login.User, Login.Password);
            }
        }
    }
}