using Newtonsoft.Json.Linq;

namespace Bizon365Extractor
{
    public class InitData
    {
        public string RoomId { get; set; }
        public string Ssid { get; set; }
        public string Ssign { get; set; }
        public string Groupid { get; set; }

        public InitData(JObject json)
        {
            if (json["roomid"] == null || json["ssid"] == null ||
                json["ssign"] == null || json["groupid"] == null)
                throw new Exception("Что то пошлло не так, ключи не найдены.\n" + json.ToString());

            RoomId = json["roomid"].ToString();
            Ssid = json["ssid"].ToString();
            Ssign = json["ssign"].ToString();
            Groupid = json["groupid"].ToString();
        }
    }
}
