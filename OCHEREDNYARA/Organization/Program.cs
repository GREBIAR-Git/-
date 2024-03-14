namespace Organization;

class Program
{
    static async Task Main()
    {
        TelegramOrganizationBot bot = new();
        await bot.Start();
    }
}