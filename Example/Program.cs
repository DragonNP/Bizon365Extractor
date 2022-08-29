using Bizon365Extractor;

while (true)
{
    string? url = Console.ReadLine();
    if (url == null)
    {
        Console.WriteLine("Введите корректную ссылку");
        continue;
    }
    string link = BizonExtractor.Extract(url).Result.HangoutsUrl;

    Console.WriteLine(link);
    break;
}