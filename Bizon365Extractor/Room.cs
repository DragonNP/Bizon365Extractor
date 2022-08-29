using Newtonsoft.Json.Linq;
using System.Linq;

namespace Bizon365Extractor
{
    public class Room
    {
        /// <summary>
        /// Название вебинара
        /// </summary>
        public string Title { get; private set; } = "";

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime Date { get; private set; } = new();

        /// <summary>
        /// Ссылка на вебинар
        /// </summary>
        public string HangoutsUrl { get; private set; } = "";

        /// <summary>
        /// Баннеры на вебинаре
        /// </summary>
        public List<Banner> Banners { get; private set; } = new();

        /// <summary>
        /// Файлы
        /// </summary>
        public List<File> Files { get; private set; } = new();

        /// <summary>
        /// Музыка
        /// </summary>
        public List<string> MusicUrls { get; private set; } = new();

        /// <summary>
        /// Автор создателя вебинара
        /// </summary>
        public string Author { get; private set; } = "";

        /// <summary>
        /// Привественное сообщение в чате
        /// </summary>
        public string WelcomeMsg { get; private set; } = "";

        /// <summary>
        /// Пользовательская галлерея
        /// </summary>
        public List<string> UserGalleryUrl { get; private set; } = new();

        public static Room FromJson(JObject json)
        {
            Console.WriteLine(json.ToString());

            Room room = new();
            JToken? value;

            value = json["title"];
            if (value != null)
            {
                room.Title = value.ToString();
            }

            value = json["date"];
            if (value != null)
            {
                room.Date = DateTime.ParseExact(value.ToString().Replace("T", " ").Replace("Z", ""),
                    "yyyyMMdd HH:mm:ss.fff",
                    System.Globalization.CultureInfo.InvariantCulture);
            }

            value = json["hangoutsUrl"];
            if (value != null)
            {
                room.HangoutsUrl = value.ToString();

                value = json["name"];
                if (room.HangoutsUrl.Contains("BizonStream: ") && value != null)
                {
                    string groupId = value.ToString().Split(':')[0];
                    room.HangoutsUrl = ConvertBizonVideo(room.HangoutsUrl, groupId);
                }
            }

            value = json["banners"];
            if (value != null)
            {
                foreach (var banner_json in JArray.Parse(value.ToString()))
                    room.Banners.Add(Banner.FromJson((JObject)banner_json));
            }

            value = json["files"];
            if (value != null)
            {
                foreach (var file_json in JArray.Parse(value.ToString()))
                    room.Files.Add(File.FromJson((JObject)file_json));
            }

            value = json["music"];
            if (value != null)
            {
                foreach (var music_json in JArray.Parse(value.ToString()))
                {
                    value = music_json["url"];
                    if (value == null) continue;
                    room.MusicUrls.Add(value.ToString());
                }
            }

            value = json["author"];
            if (value != null)
                room.Author = value.ToString();

            value = json["welcomemsg"];
            if (value != null)
                room.WelcomeMsg = value.ToString();

            value = json["userGallery"];
            if (value != null)
            {
                foreach (var gallery_json in JArray.Parse(value.ToString()))
                {
                    value = gallery_json["url"];
                    if (value == null) continue;
                    room.UserGalleryUrl.Add(value.ToString());
                }
            }
            return room;
        }

        private static string ConvertBizonVideo(string hangoutsUrl, string groupId)
        {
            return $"https://start.bizon365.ru/player/bizon/?group={groupId}&v={hangoutsUrl.Replace("BizonStream: ", "")}";
        }
    }

    public class File
    {
        public string Url { get; private set; } = "";
        public string Title { get; private set; } = "";

        public static File FromJson(JObject json)
        {
            File file = new();
            JToken? value;

            value = json["url"];
            if (value != null)
                file.Url = value.ToString();

            value = json["title"];
            if (value != null)
                file.Title = value.ToString();

            return file;
        }

    }

    public class Banner
    {
        public string Url { get; private set; } = "";
        public string Title { get; private set; } = "";
        public string Image { get; private set; } = "";

        public static Banner FromJson(JObject json)
        {
            Banner banner = new();
            JToken? value;

            value = json["url"];
            if (value != null)
                banner.Url = value.ToString();

            value = json["title"];
            if (value != null)
                banner.Title = value.ToString();

            value = json["image"];
            if (value != null)
                banner.Image = value.ToString();

            return banner;
        }
    }
}
