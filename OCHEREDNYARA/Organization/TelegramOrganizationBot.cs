using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot;

internal class TelegramOrganizationBot
{
    readonly string token;

    readonly ITelegramBotClient bot;

    readonly ReceiverOptions receiverOptions;

    readonly Organization organization;

    public TelegramOrganizationBot()
    {
        token = "";
        organization = new();
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

                Chat chat = message.Chat;

                switch (message.Type)
                {
                    case MessageType.Text:
                        {
                            if (message.Text == "/signup")
                            {
                                await organization.Theme0(user.Id);
                                await organization.SingUp(botClient, chat.Id, user.Id);
                            }
                            else if (message.Text == "/getidentificator")
                            {
                                await organization.Theme0(user.Id);
                                await organization.GetIdentificator(botClient, chat.Id, user.Id);
                            }
                            else if (message.Text == "/createroom")
                            {
                                await organization.Theme0(user.Id);
                                await organization.AddRoom1(botClient, chat.Id, user.Id);
                            }
                            else if (message.Text == "/changename")
                            {
                                await organization.Theme0(user.Id);
                                await organization.ChangeName1(botClient, chat.Id, user.Id);
                            }
                            else if (message.Text == "/rooms")
                            {
                                await organization.Theme0(user.Id);
                                await organization.AllRooms(botClient, chat.Id, user.Id);
                            }
                            else
                            {
                                int theme = await organization.GetTheme(user.Id);
                                if (theme == 1)
                                {
                                    await organization.AddRoom2(botClient, chat.Id, user.Id, message.Text);
                                    await organization.AllRooms(botClient, chat.Id, user.Id);
                                }
                                else if (theme == 2)
                                {
                                    await organization.ChangeRoom2(botClient, chat.Id, user.Id, message.Text);
                                    await organization.AllRooms(botClient, chat.Id, user.Id);
                                }
                                else if (theme == 3)
                                {
                                    await organization.AddWindow3(botClient, chat.Id, user.Id, message.Text);
                                }
                                else if (theme == 4)
                                {
                                    organization.AddWindow4(botClient, chat.Id, user.Id, message.Text);
                                }
                                else if (theme == 5)
                                {
                                    await organization.ChangeTime2(botClient, chat.Id, user.Id, message.Text);
                                }
                                else if (theme == 6)
                                {
                                    await organization.ChangeTime3(botClient, chat.Id, user.Id, message.Text);
                                }
                                else if (theme == 7)
                                {
                                    await organization.ChangeName2(botClient, chat.Id, user.Id, message.Text);
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
                            break;
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
            Chat chat = callbackQuery.Message.Chat;

            string[] data = callbackQuery.Data.Split("!");
            if (data.Length != 2)
            {
                return;
            }
            await organization.Theme0(user.Id);
            switch (data[0])
            {
                case "R1":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.AddQueue(botClient, chat.Id, user.Id, data[1]);
                        await organization.AllRooms(botClient, chat.Id, user.Id);
                        break;
                    }
                case "R2":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.ChangeRoom1(botClient, chat.Id, user.Id, data[1]);
                        break;
                    }
                case "R3":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.DeleteRoom(botClient, chat.Id, data[1]);
                        await organization.AllRooms(botClient, chat.Id, user.Id);
                        break;
                    }
                case "R4":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.AllQueueInRoom(botClient, chat.Id, user.Id, data[1]);
                        break;
                    }
                case "R5":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.FreezAllQueueInRoom(botClient, chat.Id, data[1]);
                        await organization.AllRooms(botClient, chat.Id, user.Id);
                        break;
                    }
                case "R6":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.AddWindow1(botClient, chat.Id, user.Id, data[1]);
                        break;
                    }
                case "R7":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.DeleteQueue(botClient, chat.Id, data[1]);
                        break;
                    }
                case "R8":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.FreezUnfreez(botClient, chat.Id, data[1]);
                        break;
                    }
                case "R9":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.AllWindowsInQueue(botClient, chat.Id, data[1]);
                        break;
                    }
                case "R10":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.AddWindow2(botClient, chat.Id, user.Id, data[1]);
                        break;
                    }
                case "R11":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.ChangeTime1(botClient, chat.Id, user.Id, data[1]);
                        break;
                    }
                case "R12":
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                        await organization.DeleteWindow(botClient, chat.Id, data[1]);
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

    Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
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

