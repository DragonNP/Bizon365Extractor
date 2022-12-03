using Bizon365Extractor;

//string? url = Console.ReadLine();\
string url = "https://start.bizon365.ru/room/nadintort/FerreroRoshe";
Room room = BizonExtractor.Extract(url).Result;


Console.WriteLine(room.HangoutsUrl);