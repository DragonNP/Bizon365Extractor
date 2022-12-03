using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Bizon365Extractor;

namespace TelegramBot
{
    class Program
    {
        static ITelegramBotClient bot;
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                string text = message.Text;
                if (text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Привет! Отправте мне ссылку на идущий вебинар bizon365 и получите ссылку на YouTube");
                    return;
                }
                else if (IsURL(FormatURL(text.ToLower())))
                {
                    string url = FormatURL(text);
                    Room room = BizonExtractor.Extract(url).Result;

                    await botClient.SendTextMessageAsync(message.Chat, ConvertRoomToText(room), Telegram.Bot.Types.Enums.ParseMode.Html, null, true);
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Пожалуйста, отправте ссылку на bizon365 ");
                }

            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
            string? token = Environment.GetEnvironmentVariable("TELEGRAM_TOKEN");

            if (token == null)
            {
                Console.WriteLine("Пожалуйста, укажите TELEGRAM_TOKEN");
                return;
            }

            bot = new TelegramBotClient(token);

            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }

        private static string ConvertRoomToText(Room room)
        {
            string name = room.Title;
            string date = room.Date.ToString("dd.MM.yy HH:mm");
            string link = room.HangoutsUrl;
            // Banners
            // Files
            // Musics
            string author = room.Author;
            // UserGallery

            string text = $"<b>{name}</b>\n\n<b>Автор:</b> {author}\n<b>Дата проведения:</b> {date}\n<b>Ссылка на вебинар:</b> {link}";

            return text;
        }

        private static bool IsURL(string text)
        {

            if (!text.StartsWith("https://start.bizon365.ru"))
                return false;

            return true;
        }

        private static string FormatURL(string url)
        {
            string text = url;

            if (!text.StartsWith("http://") || !text.StartsWith("https://"))
            {
                text = text.Replace("https://", "");
                text = text.Replace("http://", "");
                text = "https://" + text;
            }

            return text;
        }
    }
}