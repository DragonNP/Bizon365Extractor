using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Bizon365Extractor;

namespace TelegramBot
{
    class Program
    {
        const string TELEGRAM_TOKEN = "5799113925:AAGqQGN-Wjxnb-rmCPyl9nOd6wrDkcfGddo";

        static ITelegramBotClient bot = new TelegramBotClient(TELEGRAM_TOKEN);
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать на борт, добрый путник!");
                    return;
                }

                string url = message.Text;
                Room room = BizonExtractor.Extract(url).Result;

                await botClient.SendTextMessageAsync(message.Chat, ConvertRoomToText(room), Telegram.Bot.Types.Enums.ParseMode.Html,null,true);
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Некоторые действия
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }


        static void Main(string[] args)
        {
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
    }
}