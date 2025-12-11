using Newtonsoft.Json;
using System.ComponentModel;
using static P3tr0viCh.Utils.PasswordProperty;

namespace P3tr0viCh.Utils
{
    [TypeConverter(typeof(PropertySortedConverter))]
    [LocalizedAttribute.DisplayName("Login.DisplayName", LocalizedAttributes.ResourceName)]
    public class Login
    {
        [PropertyOrder(100)]
        [LocalizedAttribute.DisplayName("Login.User.DisplayName", LocalizedAttributes.ResourceName)] 
        public string User { get; set; }

        [PropertyOrder(101)]
        [PasswordPropertyText(true)]
        [JsonConverter(typeof(PasswordConverter))]
        [LocalizedAttribute.DisplayName("Login.Password.DisplayName", LocalizedAttributes.ResourceName)]
        public string Password { get; set; }
    }
}