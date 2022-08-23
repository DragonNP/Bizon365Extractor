using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bizon365Extractor
{
    public class Room
    {
        /// <summary>
        /// Название вебинара
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime Date { get; }

        /// <summary>
        /// Ссылка на вебинар
        /// </summary>
        public string HangoutsUrl { get; }

        /// <summary>
        /// Баннеры на вебинаре
        /// </summary>
        public List<Banner> Banners { get; }

        /// <summary>
        /// Файлы
        /// </summary>
        public List<File> Files { get; }

        /// <summary>
        /// Автор создателя вебинара
        /// </summary>
        public string Author { get; }

        /// <summary>
        /// Привественное сообщение в чате
        /// </summary>
        public string WelcomeMsg { get; }

        private Room(string title, DateTime date, string hangoutsUrl, List<Banner> banners, List<File> files, string author, string welcomeMsg)
        {
            Title = title;
            Date = date;
            HangoutsUrl = hangoutsUrl;
            Banners = banners;
            Files = files;
            Author = author;
            WelcomeMsg = welcomeMsg;
        }

        public static Room FromJson(JObject json)
        {
            string title, hangoutsUrl, author, welcomeMsg;
            DateTime date = new();
            List<Banner> banners = new();
            List<File> files = new();

            if (json["title"] != null)
                title = json["title"].ToString();
            else
                throw new Exception("title не найден " + json.ToString());

            if (json["hangoutsUrl"] != null)
            {
                hangoutsUrl = json["hangoutsUrl"].ToString();

                if (hangoutsUrl.Contains("BizonStream: "))
                {
                    string groupId = json["name"].ToString().Split(':')[0];
                    hangoutsUrl = ConvertBizonVideo(hangoutsUrl, groupId);
                }
            }
            else
                throw new Exception("hangoutsUrl не найден " + json.ToString());

            if (json["author"] != null)
                author = json["author"].ToString();
            else
                throw new Exception("author не найден " + json.ToString());

            if (json["welcomemsg"] != null)
                welcomeMsg = json["welcomemsg"].ToString();
            else
                throw new Exception("welcomemsg не найден " + json.ToString());

            /*if (json["date"] != null)
                date = DateTime.Parse(json["date"].ToString().Replace("T", " ").Replace("Z", " "));
            else
                throw new Exception("date не найден " + json.ToString());*/

            // Banners

            // Files

            return new Room(title, date, hangoutsUrl, banners, files, author, welcomeMsg);
        }

        private static string ConvertBizonVideo(string hangoutsUrl, string groupId)
        {
            return $"https://start.bizon365.ru/player/bizon/?group={groupId}&v={hangoutsUrl.Replace("BizonStream: ", "")}";
        }
    }

    public class File
    {
        public string Name { get; }
        public string Title { get; }

        public File FromJson(JObject json)
        {
            throw new NotImplementedException();
        }

    }

    public class Banner
    {
        public string Url { get; }
        public string Title { get; }

        public string Image { get; }

        public Banner FromJson(JObject json)
        {
            throw new NotImplementedException();
        }
    }
}
