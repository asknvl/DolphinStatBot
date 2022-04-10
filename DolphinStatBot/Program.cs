using DolphinStatBot.TG;
class Program
{
    public static async Task Main(string[] args)
    {

        Bot bot = new Bot();
        bot.Start();
        Console.ReadLine();
    }
}