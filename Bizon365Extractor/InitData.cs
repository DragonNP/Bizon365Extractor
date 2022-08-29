using Newtonsoft.Json.Linq;

namespace Bizon365Extractor
{
    public class InitData
    {
        public string RoomId { get; set; } = "";
        public string Ssid { get; set; } = "";
        public string Ssign { get; set; } = "";
        public string Groupid { get; set; } = "";

        public InitData(JObject json)
        {
            JToken? value;

            value = json["roomid"];
            if (value != null)
                RoomId = value.ToString();

            value = json["ssid"];
            if (value != null)
                Ssid = value.ToString();

            value = json["ssign"];
            if (value != null)
                Ssign = value.ToString();

            value = json["groupid"];
            if (value != null)
                Groupid = value.ToString();
        }
    }
}
