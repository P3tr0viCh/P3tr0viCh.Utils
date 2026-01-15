using Newtonsoft.Json;
using P3tr0viCh.Utils.Attributes;
using P3tr0viCh.Utils.Converters;
using P3tr0viCh.Utils.Properties;
using System.ComponentModel;
using static P3tr0viCh.Utils.PasswordProperty;

namespace P3tr0viCh.Utils
{
    [TypeConverter(typeof(PropertySortedConverter))]
    [LocalizedDisplayName("Login.DisplayName", Consts.ResourceName)]
    public class Login
    {
        [PropertyOrder(100)]
        [LocalizedDisplayName("Login.User.DisplayName", Consts.ResourceName)] 
        public string User { get; set; }

        [PropertyOrder(101)]
        [PasswordPropertyText(true)]
        [JsonConverter(typeof(PasswordConverter))]
        [LocalizedDisplayName("Login.Password.DisplayName", Consts.ResourceName)]
        public string Password { get; set; }
    }
}