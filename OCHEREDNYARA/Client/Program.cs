namespace TelegramBot;

class Program
{
    static async Task Main()
    {
        TelegramClientBot bot = new();
        await bot.Start();
    }
}