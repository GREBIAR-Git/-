using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot;

internal class TelegramClientBot
{
    readonly string token;

    readonly ITelegramBotClient bot;

    readonly ReceiverOptions receiverOptions;

    readonly Client client;

    public TelegramClientBot()
    {
        token = "";
        client = new();
        bot = new TelegramBotClient(token);
        receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            },
            ThrowPendingUpdates = true,
        };
    }

    public async Task Start()
    {
        using var cts = new CancellationTokenSource();
        bot.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions, cts.Token);

        var me = await bot.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1);
    }

    async Task UpdateHandlerMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        Message? message = update.Message;
        if (message != null && message.Text != null)
        {
            User? user = message.From;
            if (user != null)
            {
                Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                var chat = message.Chat;

                switch (message.Type)
                {
                    case MessageType.Text:
                        {
                            if (message.Text == "/search")
                            {
                                await client.Theme0(user.Id);
                                await client.Search1(botClient, chat.Id, user.Id);
                            }
                            else if (message.Text == "/favorites")
                            {
                                await client.Theme0(user.Id);
                                await client.GetFavorits(botClient, chat.Id, user.Id);
                            }
                            else if (message.Text == "/reserved")
                            {
                                await client.Theme0(user.Id);
                                await client.GetReserved(botClient, chat.Id, user.Id);
                            }
                            else if (message.Text == "/signup")
                            {
                                await client.Theme0(user.Id);
                                await client.SingUp(botClient, chat.Id, user.Id);
                            }
                            else
                            {
                                int theme = await client.GetTheme(user.Id);
                                if (theme == 1)
                                {
                                    await client.Search2(botClient, chat.Id, user.Id, message.Text);
                                }
                            }

                            break;
                        }
                    default:
                        {
                            await botClient.SendTextMessageAsync(
                                chat.Id,
                                "Используй только текст!",
                                cancellationToken: cancellationToken);
                            return;
                        }
                }
            }
        }
    }

    async Task UpdateHandlerCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        CallbackQuery? callbackQuery = update.CallbackQuery;
        if (callbackQuery != null && callbackQuery.Message != null && callbackQuery.Data != null)
        {
            User user = callbackQuery.From;

            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

            var chat = callbackQuery.Message.Chat;

            string[] data = callbackQuery.Data.Split("!");
            if (data.Length != 2)
            {
                return;
            }
            await client.Theme0(user.Id);
            switch (data[0])
            {
                case "ViewQueues":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                        await client.AllQueueInRoom(botClient, chat.Id, user.Id, data[1]);
                        break;
                    }
                case "AddToFavorites":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                        await client.AddFavorits(botClient, chat.Id, user.Id, long.Parse(data[1]));
                        await client.GetFavorits(botClient, chat.Id, user.Id);
                        break;
                    }
                case "RemoveFromFavorites":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                        await client.DeleteFavorits(botClient, chat.Id, user.Id, long.Parse(data[1]));
                        await client.GetFavorits(botClient, chat.Id, user.Id);

                        break;
                    }
                case "ViewQueue":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                        await client.AllWindowInQueue(botClient, chat.Id, data[1]);

                        break;
                    }
                case "BookWindow":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                        await client.AllWindowInQueue1(botClient, chat.Id, long.Parse(data[1]));

                        break;
                    }
                case "AddReserved":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        string[] r6Data = data[1].Split("#");
                        DateTime date = DateTime.Parse(r6Data[1]);
                        await client.AddReserveds(botClient, chat.Id, user.Id, long.Parse(r6Data[0]), date);

                        break;
                    }
                case "DeleteRecord":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await client.DeleteReserved(botClient, chat.Id, user.Id, long.Parse(data[1]));
                        break;
                    }
            }
        }
    }

    async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        await UpdateHandlerMessage(botClient, update, cancellationToken);
                        return;
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

    static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
