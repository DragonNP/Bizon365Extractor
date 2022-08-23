using Bizon365Extractor;

while (true)
{
    //string? url = Console.ReadLine();
    string url = "https://start.bizon365.ru/room/studiarospis/l6wbxm5u10";
    //string url = "https://start.bizon365.ru/room/3913/Dorogo2";
    if (url == null)
    {
        Console.WriteLine("Введите корректную ссылку");
        continue;
    }
    string link = BizonExtractor.ExtractLink(url).Result;

    Console.WriteLine(link);
    break;
}