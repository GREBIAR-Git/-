namespace Client;

class Program
{
    static async Task Main()
    {
        TelegramClientBot bot = new();
        await bot.Start();
    }
}