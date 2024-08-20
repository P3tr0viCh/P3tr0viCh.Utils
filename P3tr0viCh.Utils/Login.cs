using Newtonsoft.Json;
using System.ComponentModel;
using static P3tr0viCh.Utils.PasswordProperty;

namespace P3tr0viCh.Utils
{
    [TypeConverter(typeof(PropertySortedConverter))]
    public class Login
    {
        [PropertyOrder(100)] 
        public string User { get; set; }

        [PropertyOrder(101)]
        [JsonConverter(typeof(PasswordConverter))]
        [PasswordPropertyText(true)]
        public string Password { get; set; }
    }
}