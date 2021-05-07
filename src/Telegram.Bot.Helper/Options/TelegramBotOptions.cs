using System.Reflection;
#if NET5_0 || NETCOREAPP3_1
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#endif

namespace Telegram.Bot.Helper.Options
{
    public sealed class TelegramBotOptions
    {
#if NET5_0 || NETCOREAPP3_1
        [JsonConverter(typeof(JsonStringEnumConverter))]
#else
        [JsonConverter(typeof(StringEnumConverter))]
#endif
        public string[] AllowedUpdates { get; set; }
        
        [JsonIgnore]
        public Assembly[] Assemblies { get; set; }
    }
}