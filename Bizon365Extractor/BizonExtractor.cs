using System.Text;
using Newtonsoft.Json.Linq;

namespace Bizon365Extractor
{
    /// <summary>
    /// Статический класс для извлечения ссылки на вебинар из сервиса Bizon365 
    /// </summary>
    public static class BizonExtractor
    {
        /// <summary>
        /// Асинхронный метод извлекающий ссылку на вебинар
        /// </summary>
        /// <param name="url">Ссылка на идущий вебинар</param>
        /// <returns>Ссылка на вебинар в других виеохостингах (YouTube и т.д.)</returns>
        public static async Task<Room> Extract(string url)
        {
            // Получаем специальный cookie
            string sid = GetSidAsync(url).Result;

            // Логика извечения ссылки из страницы
            InitData initData = await LoadInitData(url, sid);
            string sid_special = await GetSidForLink(initData);
            Room room = await Final(initData, sid_special);

            return room;
        }

        private static async Task<InitData> LoadInitData(string url, string sid)
        {
            string url_loadInitData = url + "/loadInitData";

            HttpClient client = new();

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url_loadInitData);
            requestMessage.Headers.Add("host", "start.bizon365.ru");
            requestMessage.Headers.Add("cookie", $"sid={sid}");


            requestMessage.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await client.SendAsync(requestMessage);

            JObject json = JObject.Parse(await response.Content.ReadAsStringAsync());
            return new InitData(json);
        }

        private static async Task<string> GetSidForLink(InitData initData)
        {
            string url = "https://ws5.bizon365.ru/socket.io/?";
            url += $"ssid={initData.Ssid}&";
            url += $"ssign={initData.Ssign}&";
            url += $"roomid={initData.RoomId}&";
            url += $"group={initData.Groupid}&";
            url += "transport=polling";

            HttpClient client = new();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("host", "ws5.bizon365.ru");
            var response = await client.SendAsync(requestMessage);

            string content = await response.Content.ReadAsStringAsync();

            while (content[0] != '{')
                content = content.Remove(0, 1);

            JToken? sid = JObject.Parse(content)["sid"];
            if (sid == null)
                throw new Exception("Sid не найден. " + content);

            return sid.ToString();
        }

        private static async Task<Room> Final(InitData initData, string sid)
        {
            string url = "https://ws5.bizon365.ru/socket.io/?";
            url += $"ssid={initData.Ssid}&";
            url += $"ssign={initData.Ssign}&";
            url += $"roomid={initData.RoomId}&";
            url += $"group={initData.Groupid}&";
            url += $"sid={sid}&";
            url += "transport=polling";

            HttpClient client = new();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("host", "ws5.bizon365.ru");
            var response = await client.SendAsync(requestMessage);

            string content = await response.Content.ReadAsStringAsync();

            content = RemoveStrangeSymbols(content);

            while (content[0] != '{')
                content = content.Remove(0, 1);
            content = content.Remove(content.Length - 1, 1);

            JObject json = JObject.Parse(content);
            JToken? room_json = json["room"];

            if (room_json == null) return new Room();
            Room room = Room.FromJson((JObject)room_json);
            return room;
        }

        /// <summary>
        /// Асинхронный метод получает специальный авторизованный cookie
        /// </summary>
        /// <param name="url">Ссылка на идущий вебинар</param>
        /// <returns>Sid cookie</returns>
        private static async Task<string> GetSidAsync(string url)
        {
            if (url[url.Length - 1] != '/') url += "/";
            string url_authorize = url + "authorize";

            string body = "{ \"username\": \"test\", " +
                "\"email\": \"test@email.com\", " +
                "\"phone\": \"+71234567890\", " +
                "\"custom1\": \"test\", " +
                "\"referer\": \"test\", " +
                "\"param1\": \"test\", " +
                "\"param2\": \"test\", " +
                "\"param3\": \"test\", " +
                "\"cu1\": \"test\", " +
                "\"sup\": \"test\" }";

            HttpClient client = new();

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url_authorize);
            requestMessage.Headers.Add("host", "start.bizon365.ru");
            requestMessage.Content = new StringContent(body, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(requestMessage);

            string sid_cookie = "";
            foreach (var item in response.Headers.GetValues("Set-Cookie"))
            {
                string[] cookie = item.Split(';')[0].Split('=');
                if (cookie[0] == "sid")
                {
                    sid_cookie = cookie[1];
                    break;
                }
            }
            return sid_cookie;
        }

        private static string RemoveStrangeSymbols(string line)
        {
            string result = "";

            foreach (char symbol in line)
            {
                char[] chars = new char[] { '{', '}', ',', '.', '/', '\\', '!', '@', '#', '$', '%', '^', '&', '*', '\'', '\"', ';', '_', '(', ')', ':', '|', '[', ']' };

                if (chars.Contains(symbol) || 
                    (symbol >= 'А' && symbol <= 'Я') || (symbol >= 'а' && symbol <= 'я') || 
                    (symbol >= 'A' && symbol <= 'Z') || (symbol >= 'a' && symbol <= 'z') ||
                    (symbol >= '0' && symbol <= '9') || symbol == ' ')
                {
                    result += symbol;
                }
            }
            result = result.Replace("4042", "");
            return result;
        }
    }
}