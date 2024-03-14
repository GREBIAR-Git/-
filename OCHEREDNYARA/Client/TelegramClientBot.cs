using DataBase.Context;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;
using YamlDotNet.RepresentationModel;

namespace Client;

class TelegramClientBot : TelegramBotBase
{
    public TelegramClientBot()
    {
        YamlMappingNode yaml = ReadingYaml();

        string token = yaml["Token"].ToString();
        Bot = new TelegramBotClient(token);
        ContextDataBase.ConnectingString = yaml["ConnectionString"].ToString();
        if (yaml["DataReloading"].ToString() == "Yes")
        {
            MainDataBase.ResetDB();
        }
    }

    protected override async Task UpdateHandlerMessage(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        Message? message = update.Message;
        if (message?.Text != null)
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
                        if (message.Text == "/search")
                        {
                            await ClientInteractionDB.Theme0(user.Id);
                            await ClientInteractionDB.Search1(botClient, chat.Id, user.Id);
                        }
                        else if (message.Text == "/favorites")
                        {
                            await ClientInteractionDB.Theme0(user.Id);
                            await ClientInteractionDB.GetFavorites(botClient, chat.Id, user.Id);
                        }
                        else if (message.Text == "/reserved")
                        {
                            await ClientInteractionDB.Theme0(user.Id);
                            await ClientInteractionDB.GetReserved(botClient, chat.Id, user.Id);
                        }
                        else if (message.Text == "/signup")
                        {
                            await ClientInteractionDB.Theme0(user.Id);
                            await ClientInteractionDB.SingUp(botClient, chat.Id, user.Id);
                        }
                        else
                        {
                            int theme = await ClientInteractionDB.GetTheme(user.Id);
                            if (theme == 1)
                            {
                                await ClientInteractionDB.Search2(botClient, chat.Id, user.Id, message.Text);
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

    protected override async Task UpdateHandlerCallbackQuery(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        CallbackQuery? callbackQuery = update.CallbackQuery;
        if (callbackQuery is { Message: not null, Data: not null })
        {
            User user = callbackQuery.From;

            Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");

            Chat chat = callbackQuery.Message.Chat;

            string[] data = callbackQuery.Data.Split("!");
            if (data.Length != 2)
            {
                return;
            }

            await ClientInteractionDB.Theme0(user.Id);
            switch (data[0])
            {
                case "ViewQueues":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                    await ClientInteractionDB.AllQueueInRoom(botClient, chat.Id, user.Id, data[1]);
                    break;
                }
                case "AddToFavorites":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                    await ClientInteractionDB.AddFavorites(botClient, chat.Id, user.Id, long.Parse(data[1]));
                    await ClientInteractionDB.GetFavorites(botClient, chat.Id, user.Id);
                    break;
                }
                case "RemoveFromFavorites":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                    await ClientInteractionDB.DeleteFavorites(botClient, chat.Id, user.Id, long.Parse(data[1]));
                    await ClientInteractionDB.GetFavorites(botClient, chat.Id, user.Id);

                    break;
                }
                case "ViewQueue":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                    await ClientInteractionDB.AllWindowInQueue(botClient, chat.Id, data[1]);

                    break;
                }
                case "BookWindow":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

                    await ClientInteractionDB.AllWindowInQueue1(botClient, chat.Id, long.Parse(data[1]));

                    break;
                }
                case "AddReserved":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    string[] r6Data = data[1].Split("#");
                    DateTime date = DateTime.Parse(r6Data[1]);
                    await ClientInteractionDB.AddReserveds(botClient, chat.Id, user.Id, long.Parse(r6Data[0]), date);

                    break;
                }
                case "DeleteRecord":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await ClientInteractionDB.DeleteReserved(botClient, chat.Id, user.Id, long.Parse(data[1]));
                    break;
                }
            }
        }
    }
}