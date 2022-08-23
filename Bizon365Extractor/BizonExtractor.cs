using System.Text;

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
        public static async Task<string> ExtractLink(string url)
        {
            // Получаем специальный cookie
            string sid = GetSidAsync(url).Result;

            // Получаем HTML страницу
            string html_page = await GetHtmlResponse(url, sid);

            // Логика извечения ссылки из страницы

            return html_page;
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

        /// <summary>
        /// Асинхронный метод подает запрос на страницу вебинара
        /// </summary>
        /// <param name="url">Ссылка на идущий вебинар</param>
        /// <param name="sid">Специальный авторизованный cookie</param>
        /// <returns>HTML страница вебинара</returns>
        private static async Task<string> GetHtmlResponse(string url, string sid)
        {
            HttpClient client = new();

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            requestMessage.Headers.Add("cookie", $"sid={sid}");
            var responseMessage = await client.SendAsync(requestMessage);

            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}