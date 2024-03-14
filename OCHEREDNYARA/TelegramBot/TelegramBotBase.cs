using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YamlDotNet.RepresentationModel;

namespace TelegramBot;

public abstract class TelegramBotBase
{
    protected readonly ReceiverOptions ReceiverOptions = new()
    {
        AllowedUpdates =
        [
            UpdateType.Message,
            UpdateType.CallbackQuery
        ],
        ThrowPendingUpdates = true
    };

    protected ITelegramBotClient? Bot;

    protected static YamlMappingNode ReadingYaml()
    {
        using StreamReader reader = new("config.yml");
        YamlStream yaml = [];
        yaml.Load(reader);
        YamlMappingNode root = (YamlMappingNode)yaml.Documents[0].RootNode;
        return root;
    }

    public virtual async Task Start()
    {
        using CancellationTokenSource cts = new();
        if (Bot != null)
        {
            Bot.StartReceiving(UpdateHandler, ErrorHandler, ReceiverOptions, cts.Token);

            User me = await Bot.GetMeAsync();
            Console.WriteLine($"{me.FirstName} запущен!");
        }

        await Task.Delay(-1);
    }

    protected abstract Task UpdateHandlerCallbackQuery(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken);

    protected abstract Task UpdateHandlerMessage(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken);

    protected virtual Task ErrorHandler(ITelegramBotClient botClient, Exception error,
        CancellationToken cancellationToken)
    {
        string errorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    protected virtual async Task UpdateHandler(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                {
                    await UpdateHandlerMessage(botClient, update, cancellationToken);
                    break;
                }
                case UpdateType.CallbackQuery:
                {
                    await UpdateHandlerCallbackQuery(botClient, update, cancellationToken);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}