using Newtonsoft.Json;
using P3tr0viCh.Utils.Properties;
using System.ComponentModel;
using static P3tr0viCh.Utils.Converters;

namespace P3tr0viCh.Utils
{
    public class DataBaseConnection
    {
        public interface IConnection
        {
            string ConnectionString { get; }
        }

        [TypeConverter(typeof(PropertySortedConverter))]
        [LocalizedAttribute.DisplayName("Connection.DisplayName", "Properties.Resources.Utils")]
        public abstract class Connection : IConnection
        {
            [PropertyOrder(300)]
            public Login Login { get; set; } = new Login();

            [JsonIgnore]
            [Browsable(false)]
            public abstract string ConnectionString { get; }
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

            public const bool DefaultUseSsl = true;

            [PropertyOrder(300)]
            [TypeConverter(typeof(BooleanTypeOnOffConverter))]
            [LocalizedAttribute.DisplayName("Connection.UseSsl.DisplayName", "Properties.Resources.Utils")]
            public bool UseSsl { get; set; }

            public ConnectionMySql()
            {
                Host = DefaultHost;
                Port = DefaultPort;

                UseSsl = DefaultUseSsl;
            }

            public override string ConnectionString =>
                string.Format(Resources.ConnectionStringMySql,
                    Host.IsEmpty() ? DefaultHost : Host,
                    Port == 0 ? DefaultPort : Port,
                    Database,
                    Login.User, Login.Password,
                    UseSsl ? Resources.ConnectionStringMySqlSslPreferred : Resources.ConnectionStringMySqlSslNone);
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

            public override string ConnectionString =>
                string.Format(Resources.ConnectionFireBird,
                    Host.IsEmpty() ? DefaultHost : Host,
                    Port == 0 ? DefaultPort : Port,
                    Database,
                    Login.User, Login.Password);
        }
    }
}