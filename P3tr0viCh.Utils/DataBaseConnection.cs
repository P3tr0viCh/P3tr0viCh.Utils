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
        public abstract class Connection : IConnection
        {
            [PropertyOrder(300)]
            public Login Login { get; set; } = new Login();

            public abstract string ConnectionString();
        }

        public abstract class ConnectionServer : Connection
        {
            [PropertyOrder(100)]
            public string Host { get; set; }

            [PropertyOrder(101)]
            public int Port { get; set; } = 0;

            [PropertyOrder(400)]
            public string Driver { get; set; }
        }

        public class ConnectionMySql : ConnectionServer
        {
            public const string DefaultHost = "localhost";
            public const int DefaultPort = 3306;

            public ConnectionMySql()
            {
                Host = DefaultHost;
                Port = DefaultPort;
            }

            [PropertyOrder(200)]
            public string Database { get; set; }

            public override string ConnectionString()
            {
                return string.Format(Resources.ConnectionStringMySql,
                    Host.IsEmpty() ? DefaultHost : Host,
                    Port == 0 ? DefaultPort : Port,
                    Database,
                    Login.User, Login.Password,
                    Driver);
            }
        }
    }
}