using DataBase.Context;
using DataBase.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Organization;

class OrganizationInteractionDB
{
    public static async Task Theme0(long idOrganization)
    {
        await MainDataBase.ChangeThemeOrganization(idOrganization, 0);
    }

    public static async Task<int> GetTheme(long idOrganization)
    {
        return await MainDataBase.GetThemeOrganization(idOrganization);
    }

    public static async Task GetIdentificator(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        if (await OrganizationDataBase.IsOrganizationActive(idOrganization))
        {
            await botClient.SendTextMessageAsync(idChat, idOrganization.ToString());
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Вы не зарегистрирвоанны");
        }
    }

    public static async Task SingUp(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        if (await OrganizationDataBase.AddOrganization(idOrganization, "Неизвестное"))
        {
            await botClient.SendTextMessageAsync(idChat, "Ваша организация успешно зарегистрирвоана");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Ваша организация уже зарегистрирована");
        }
    }

    public static async Task ChangeName1(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        await botClient.SendTextMessageAsync(idChat, "Введите название организации");
        await MainDataBase.ChangeThemeOrganization(idOrganization, 7);
    }

    public static async Task ChangeName2(ITelegramBotClient botClient, long idChat, long idOrganization, string name)
    {
        await MainDataBase.ChangeThemeOrganization(idOrganization, 0);
        if (await OrganizationDataBase.EditOrganizationName(idOrganization, name))
        {
            await botClient.SendTextMessageAsync(idChat, "Ваше имя успешно изменено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public static async Task AllRooms(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        await MainDataBase.ChangeThemeOrganization(idOrganization, 0);
        List<Room> rooms = await RoomDataBase.GetRooms(idOrganization);

        string text = "Список ваших комнат";

        await botClient.SendTextMessageAsync(
            idChat,
            text);

        for (int i = 0; i < rooms.Count; i++)
        {
            InlineKeyboardMarkup ikm = new(new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData("Добавить очередь", "R1!" + rooms[i].Id),
                    InlineKeyboardButton.WithCallbackData("Изменить", "R2!" + rooms[i].Id),
                    InlineKeyboardButton.WithCallbackData("Удалить", "R3!" + rooms[i].Id)
                ],
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Посмотреть очереди", "R4!" + rooms[i].Id),
                    InlineKeyboardButton.WithCallbackData("Заморозка всех очередей", "R5!" + rooms[i].Id)
                }
            });

            string header = "№" + (i + 1) + " Название комнаты: " + rooms[i].Name + "; Количесвто очередей: ";
            if (rooms[i].Queues is not null)
            {
                header += rooms[i].Queues.Count;
            }
            else
            {
                header += "0";
            }

            await botClient.SendTextMessageAsync(idChat, header, replyMarkup: ikm);
        }
    }


    public static async Task AddQueue(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoom)
    {
        if (await QueueDataBase.AddQueues(idOrganization, long.Parse(idRoom)))
        {
            await botClient.SendTextMessageAsync(idChat, "Очередь успешно добавлена");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public static async Task ChangeRoom1(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoom)
    {
        await MainDataBase.ChangeThemeOrganization(idOrganization, 2);
        await MainDataBase.ChangeThemeDataOrganization(idOrganization, idRoom);
        await botClient.SendTextMessageAsync(idChat, "Введите новое имя");
    }

    public static async Task ChangeRoom2(ITelegramBotClient botClient, long idChat, long idOrganization, string newName)
    {
        string data = await MainDataBase.GetThemeDataOrganization(idOrganization);
        if (await RoomDataBase.EditRoomName(long.Parse(data), newName))
        {
            await botClient.SendTextMessageAsync(idChat, "Команта успешно переименована!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public static async Task AddRoom1(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        await MainDataBase.ChangeThemeOrganization(idOrganization, 1);
        await botClient.SendTextMessageAsync(idChat, "Пожалуйста, введите название комнаты");
    }

    public static async Task AddRoom2(ITelegramBotClient botClient, long idChat, long idOrganization, string name)
    {
        if (await RoomDataBase.AddRooms(idOrganization, name))
        {
            await botClient.SendTextMessageAsync(idChat, "Команта успешно создана!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public static async Task DeleteRoom(ITelegramBotClient botClient, long idChat, string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        if (await RoomDataBase.DeleteRoom(idRoom))
        {
            await botClient.SendTextMessageAsync(idChat, "Комната успешно удалена");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public static async Task AllQueueInRoom(ITelegramBotClient botClient, long idChat, long idOrganization,
        string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        await MainDataBase.ChangeThemeOrganization(idOrganization, 3);
        List<Queue> queues = await QueueDataBase.GetQueues(idRoom);
        Room? room = await RoomDataBase.GetRoom(idRoom);
        if (room != null)
        {
            string text = "Список ваших очередей в комате \"" + room.Name;

            await botClient.SendTextMessageAsync(idChat, text);

            for (int i = 0; i < queues.Count; i++)
            {
                InlineKeyboardButton inlineKeyboardFreeze =
                    InlineKeyboardButton.WithCallbackData(queues[i].Freezing ? "Разморозить" : "Заморозить",
                        "R8!" + queues[i].Id);

                InlineKeyboardMarkup ikm = new(new[]
                {
                    [
                        InlineKeyboardButton.WithCallbackData("Добавить окно", "R6!" + queues[i].Id),
                        InlineKeyboardButton.WithCallbackData("Удалить", "R7!" + queues[i].Id)
                    ],
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Посмотреть окна", "R9!" + queues[i].Id),
                        inlineKeyboardFreeze
                    }
                });
                string header = "Очередь №" + (i + 1) + "; Количесвто окон: ";
                if (queues[i].Windows is not null)
                {
                    header += queues[i].Windows.Count;
                }
                else
                {
                    header += "0";
                }

                await botClient.SendTextMessageAsync(idChat, header, replyMarkup: ikm);
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public static async Task FreezeAllQueueInRoom(ITelegramBotClient botClient, long idChat, string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        Room? room = await RoomDataBase.GetRoom(idRoom);
        if (room != null)
        {
            foreach (Queue queue in room.Queues)
            {
                await QueueDataBase.Freeze(queue.Id);
            }

            await botClient.SendTextMessageAsync(idChat, "Всё заморожено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public static async Task AddWindow1(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoomS)
    {
        InlineKeyboardMarkup ikm = new(new[]
        {
            [
                InlineKeyboardButton.WithCallbackData("Понедельник", "R10!1")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Вторник", "R10!2")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Среда", "R10!3")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Четверг", "R10!4")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Пятница", "R10!5")
            ],
            [
                InlineKeyboardButton.WithCallbackData("Суббота", "R10!6")
            ],
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Воскресенье", "R10!7")
            }
        });
        await MainDataBase.ChangeThemeDataOrganization(idOrganization, idRoomS);
        await botClient.SendTextMessageAsync(idChat, "Выберете день недели", replyMarkup: ikm);
    }

    public static async Task AddWindow2(ITelegramBotClient botClient, long idChat, long idOrganization,
        string idWeekDay)
    {
        await MainDataBase.ChangeThemeOrganization(idOrganization, 3);
        await botClient.SendTextMessageAsync(idChat, "Введите время начала окна");
        await MainDataBase.AddPartThemeDataOrganization(idOrganization, idWeekDay);
    }

    public static async Task AddWindow3(ITelegramBotClient botClient, long idChat, long idOrganization,
        string startTime)
    {
        await botClient.SendTextMessageAsync(idChat, "Введите время очончания окна");
        await MainDataBase.AddPartThemeDataOrganization(idOrganization, startTime);
        await MainDataBase.ChangeThemeOrganization(idOrganization, 4);
    }

    public static async void AddWindow4(ITelegramBotClient botClient, long idChat, long idOrganization, string endTime)
    {
        string[] data = (await MainDataBase.GetThemeDataOrganization(idOrganization)).Split("!");
        DateTime? start = StringToDateTime(data[2]);
        if (start != null)
        {
            DateTime? end = StringToDateTime(endTime);
            if (end != null)
            {
                if (await WindowDataBase.IsValidTime(long.Parse(data[0]), long.Parse(data[1]), start, end))
                {
                    if (await WindowDataBase.AddWindow(long.Parse(data[0]), long.Parse(data[1]), start.Value,
                            end.Value))
                    {
                        await botClient.SendTextMessageAsync(idChat, "Окно успешно добавлено");
                        await MainDataBase.ChangeThemeDataOrganization(idOrganization, "");
                        return;
                    }
                }
            }
        }

        await MainDataBase.ChangeThemeDataOrganization(idOrganization, "");
        await botClient.SendTextMessageAsync(idChat, "Окно не добавлено");
    }

    public static async Task DeleteQueue(ITelegramBotClient botClient, long idChat, string idQueueS)
    {
        long idQueue = long.Parse(idQueueS);
        if (await QueueDataBase.DeleteQueue(idQueue))
        {
            await botClient.SendTextMessageAsync(idChat, "Очередь успешно удалена");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public static async Task FreezeUnfreeze(ITelegramBotClient botClient, long idChat, string idQueueS)
    {
        long idQueue = long.Parse(idQueueS);
        if (await QueueDataBase.FreezeUnfreeze(idQueue))
        {
            await botClient.SendTextMessageAsync(idChat, "Действие успешно выполнено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public static async Task AllWindowsInQueue(ITelegramBotClient botClient, long idChat, string idWindowS)
    {
        await botClient.SendTextMessageAsync(idChat, "Окна по неделям");
        for (int i = 0; i < 7; i++)
        {
            List<Window> windows = await WindowDataBase.GetWindows(long.Parse(idWindowS), i + 1);
            WeekDay? weekDay = await WindowDataBase.GetWeek(i + 1);
            if (weekDay != null)
            {
                if (windows.Count != 0)
                {
                    await botClient.SendTextMessageAsync(idChat, "Окна на " + weekDay.Name);
                    for (int f = 0; f < windows.Count; f++)
                    {
                        InlineKeyboardMarkup ikm = new(new[]
                        {
                            [
                                InlineKeyboardButton.WithCallbackData("Изменить промежуток", "R11!" + windows[f].Id)
                            ],
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Удалить", "R12!" + windows[f].Id)
                            }
                        });
                        string header = " Промежуток №" + (f + 1) + "; с " + windows[f].Start.ToString("HH:mm tt") +
                                        " - " + windows[f].End.ToString("HH:mm tt") + ";";
                        await botClient.SendTextMessageAsync(idChat, header, replyMarkup: ikm);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(idChat, "Окна на " + weekDay.Name + " - Пусто");
                }
            }
        }
    }

    public static async Task ChangeTime1(ITelegramBotClient botClient, long idChat, long idOrganization,
        string idWindow)
    {
        await botClient.SendTextMessageAsync(idChat, "Введите новое начало окна");
        await MainDataBase.ChangeThemeDataOrganization(idOrganization, idWindow);
        await MainDataBase.ChangeThemeOrganization(idOrganization, 5);
    }

    public static async Task ChangeTime2(ITelegramBotClient botClient, long idChat, long idOrganization,
        string startTime)
    {
        await botClient.SendTextMessageAsync(idChat, "Введите время очончания окна");
        await MainDataBase.AddPartThemeDataOrganization(idOrganization, startTime);
        await MainDataBase.ChangeThemeOrganization(idOrganization, 6);
    }

    public static async Task ChangeTime3(ITelegramBotClient botClient, long idChat, long idOrganization, string endTime)
    {
        string[] data = (await MainDataBase.GetThemeDataOrganization(idOrganization)).Split("!");
        DateTime? start = StringToDateTime(data[1]);
        if (start != null)
        {
            DateTime? end = StringToDateTime(endTime);
            if (end != null)
            {
                if (await WindowDataBase.IsValidTime(long.Parse(data[0]), start, end))
                {
                    Window? window = await WindowDataBase.GetWindow(long.Parse(data[0]));
                    if (window != null)
                    {
                        if (await WindowDataBase.DeleteWindow(long.Parse(data[0])) &&
                            await WindowDataBase.AddWindow(window.QueueId, window.WeekDayId, start.Value, end.Value))
                        {
                            await botClient.SendTextMessageAsync(idChat, "Промежуток успешно изменён");
                            return;
                        }
                    }
                }
            }
        }

        await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
    }

    static DateTime? StringToDateTime(string date) //format string 14:45
    {
        string[] dateTime = date.Split(":");
        if (dateTime.Length == 2)
        {
            if (!int.TryParse(dateTime[0], out int hour) && hour > 24)
            {
                return null;
            }

            if (!int.TryParse(dateTime[1], out int minute) && minute > 60)
            {
                return null;
            }

            return new DateTime(1, 1, 1, hour, minute, 0);
        }

        return null;
    }

    public static async Task DeleteWindow(ITelegramBotClient botClient, long idChat, string idWindow)
    {
        if (await WindowDataBase.DeleteWindow(long.Parse(idWindow)))
        {
            await botClient.SendTextMessageAsync(idChat, "Окно успешно удалено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }
}