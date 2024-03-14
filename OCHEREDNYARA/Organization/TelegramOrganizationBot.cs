using DataBase.Context;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot;
using YamlDotNet.RepresentationModel;

namespace Organization;

class TelegramOrganizationBot : TelegramBotBase
{
    public TelegramOrganizationBot()
    {
        YamlMappingNode yaml = ReadingYaml();

        string token = yaml["Token"].ToString();
        ContextDataBase.ConnectingString = yaml["ConnectionString"].ToString();
        if (yaml["DataReloading"].ToString() == "Yes")
        {
            MainDataBase.ResetDB();
        }

        Bot = new TelegramBotClient(token);
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
                        if (message.Text == "/signup")
                        {
                            await OrganizationInteractionDB.Theme0(user.Id);
                            await OrganizationInteractionDB.SingUp(botClient, chat.Id, user.Id);
                        }
                        else if (message.Text == "/getidentificator")
                        {
                            await OrganizationInteractionDB.Theme0(user.Id);
                            await OrganizationInteractionDB.GetIdentificator(botClient, chat.Id, user.Id);
                        }
                        else if (message.Text == "/createroom")
                        {
                            await OrganizationInteractionDB.Theme0(user.Id);
                            await OrganizationInteractionDB.AddRoom1(botClient, chat.Id, user.Id);
                        }
                        else if (message.Text == "/changename")
                        {
                            await OrganizationInteractionDB.Theme0(user.Id);
                            await OrganizationInteractionDB.ChangeName1(botClient, chat.Id, user.Id);
                        }
                        else if (message.Text == "/rooms")
                        {
                            await OrganizationInteractionDB.Theme0(user.Id);
                            await OrganizationInteractionDB.AllRooms(botClient, chat.Id, user.Id);
                        }
                        else
                        {
                            int theme = await OrganizationInteractionDB.GetTheme(user.Id);
                            if (theme == 1)
                            {
                                await OrganizationInteractionDB.AddRoom2(botClient, chat.Id, user.Id, message.Text);
                                await OrganizationInteractionDB.AllRooms(botClient, chat.Id, user.Id);
                            }
                            else if (theme == 2)
                            {
                                await OrganizationInteractionDB.ChangeRoom2(botClient, chat.Id, user.Id, message.Text);
                                await OrganizationInteractionDB.AllRooms(botClient, chat.Id, user.Id);
                            }
                            else if (theme == 3)
                            {
                                await OrganizationInteractionDB.AddWindow3(botClient, chat.Id, user.Id, message.Text);
                            }
                            else if (theme == 4)
                            {
                                OrganizationInteractionDB.AddWindow4(botClient, chat.Id, user.Id, message.Text);
                            }
                            else if (theme == 5)
                            {
                                await OrganizationInteractionDB.ChangeTime2(botClient, chat.Id, user.Id, message.Text);
                            }
                            else if (theme == 6)
                            {
                                await OrganizationInteractionDB.ChangeTime3(botClient, chat.Id, user.Id, message.Text);
                            }
                            else if (theme == 7)
                            {
                                await OrganizationInteractionDB.ChangeName2(botClient, chat.Id, user.Id, message.Text);
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

            await OrganizationInteractionDB.Theme0(user.Id);
            switch (data[0])
            {
                case "R1":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.AddQueue(botClient, chat.Id, user.Id, data[1]);
                    await OrganizationInteractionDB.AllRooms(botClient, chat.Id, user.Id);
                    break;
                }
                case "R2":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.ChangeRoom1(botClient, chat.Id, user.Id, data[1]);
                    break;
                }
                case "R3":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.DeleteRoom(botClient, chat.Id, data[1]);
                    await OrganizationInteractionDB.AllRooms(botClient, chat.Id, user.Id);
                    break;
                }
                case "R4":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.AllQueueInRoom(botClient, chat.Id, user.Id, data[1]);
                    break;
                }
                case "R5":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.FreezeAllQueueInRoom(botClient, chat.Id, data[1]);
                    await OrganizationInteractionDB.AllRooms(botClient, chat.Id, user.Id);
                    break;
                }
                case "R6":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.AddWindow1(botClient, chat.Id, user.Id, data[1]);
                    break;
                }
                case "R7":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.DeleteQueue(botClient, chat.Id, data[1]);
                    break;
                }
                case "R8":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.FreezeUnfreeze(botClient, chat.Id, data[1]);
                    break;
                }
                case "R9":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.AllWindowsInQueue(botClient, chat.Id, data[1]);
                    break;
                }
                case "R10":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.AddWindow2(botClient, chat.Id, user.Id, data[1]);
                    break;
                }
                case "R11":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.ChangeTime1(botClient, chat.Id, user.Id, data[1]);
                    break;
                }
                case "R12":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
                    await OrganizationInteractionDB.DeleteWindow(botClient, chat.Id, data[1]);
                    break;
                }
            }
        }
    }
}