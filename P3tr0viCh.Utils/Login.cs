using Newtonsoft.Json;
using P3tr0viCh.Utils.Attributes;
using P3tr0viCh.Utils.Converters;
using P3tr0viCh.Utils.Properties;
using System.ComponentModel;

namespace P3tr0viCh.Utils
{
    [TypeConverter(typeof(PropertySortedConverter))]
    [LocalizedDisplayName("Login.DisplayName", Consts.ResourcesName)]
    public class Login
    {
        [PropertyOrder(100)]
        [LocalizedDisplayName("Login.User.DisplayName", Consts.ResourcesName)]
        public string User { get; set; }

        [PropertyOrder(101)]
        [PasswordPropertyText(true)]
        [JsonConverter(typeof(PasswordConverter))]
        [LocalizedDisplayName("Login.Password.DisplayName", Consts.ResourcesName)]
        public string Password { get; set; }
    }
}