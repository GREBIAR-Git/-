using DataBase.Context;
using DataBase.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

internal class Organization
{
    readonly ContextDataBase db;

    public Organization()
    {
        db = new();
    }

    public async Task Theme0(long idOrganization)
    {
        await db.ChangeThemeOrganization(idOrganization, 0);
    }

    public async Task<int> GetTheme(long idOrganization)
    {
        return await db.GetThemeOrganization(idOrganization);
    }

    public async Task GetIdentificator(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        if (await db.IsOrganizationActiv(idOrganization))
        {
            await botClient.SendTextMessageAsync(idChat, idOrganization.ToString());
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Вы не зарегистрирвоанны");
        }
    }

    public async Task SingUp(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        if (await db.AddOrganization(idOrganization, "Неизвестное"))
        {
            await botClient.SendTextMessageAsync(idChat, $"Ваша организация успешно зарегистрирвоана");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Ваша организация уже зарегистрирована");
        }
    }

    public async Task ChangeName1(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        await botClient.SendTextMessageAsync(idChat, $"Введите название организации");
        await db.ChangeThemeOrganization(idOrganization, 7);
    }

    public async Task ChangeName2(ITelegramBotClient botClient, long idChat, long idOrganization, string name)
    {
        await db.ChangeThemeOrganization(idOrganization, 0);
        if (await db.EditOrganizationName(idOrganization, name))
        {
            await botClient.SendTextMessageAsync(idChat, $"Ваше имя успешно изменено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так");
        }
    }

    public async Task AllRooms(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        await db.ChangeThemeOrganization(idOrganization, 0);
        List<Room> rooms = await db.GetRooms(idOrganization);

        var text = "Список ваших комнат";

        await botClient.SendTextMessageAsync(
        idChat,
        text);

        for (int i = 0; i < rooms.Count; i++)
        {
            var ikm = new InlineKeyboardMarkup(new[]
                {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Добавить очередь", "R1!" + rooms[i].Id.ToString()),
                    InlineKeyboardButton.WithCallbackData("Изменить", "R2!" + rooms[i].Id.ToString()),
                    InlineKeyboardButton.WithCallbackData("Удалить", "R3!" + rooms[i].Id.ToString()),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Посмотреть очереди", "R4!" + rooms[i].Id.ToString()),
                    InlineKeyboardButton.WithCallbackData("Заморозка всех очередей", "R5!" + rooms[i].Id.ToString()),
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


    public async Task AddQueue(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoom)
    {
        if (await db.AddQueues(idOrganization, long.Parse(idRoom)))
        {
            await botClient.SendTextMessageAsync(idChat, $"Очередь успешно добавлена");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так");
        }
    }

    public async Task ChangeRoom1(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoom)
    {
        await db.ChangeThemeOrganization(idOrganization, 2);
        await db.ChangeThemeDataOrganization(idOrganization, idRoom);
        await botClient.SendTextMessageAsync(idChat, $"Введите новое имя");
    }

    public async Task ChangeRoom2(ITelegramBotClient botClient, long idChat, long idOrganization, string newName)
    {
        string data = await db.GetThemeDataOrganization(idOrganization);
        if (await db.EditRoomName(long.Parse(data), newName))
        {
            await botClient.SendTextMessageAsync(idChat, "Команта успешно переименована!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public async Task AddRoom1(ITelegramBotClient botClient, long idChat, long idOrganization)
    {
        await db.ChangeThemeOrganization(idOrganization, 1);
        await botClient.SendTextMessageAsync(idChat, "Пожалуйста, введите название комнаты");
    }

    public async Task AddRoom2(ITelegramBotClient botClient, long idChat, long idOrganization, string name)
    {
        if (await db.AddRooms(idOrganization, name))
        {
            await botClient.SendTextMessageAsync(idChat, "Команта успешно создана!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так");
        }
    }

    public async Task DeleteRoom(ITelegramBotClient botClient, long idChat, string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        if (await db.DeleteRoom(idRoom))
        {
            await botClient.SendTextMessageAsync(idChat, $"Комната успешно удалена");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так");
        }
    }

    public async Task AllQueueInRoom(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        await db.ChangeThemeOrganization(idOrganization, 3);
        List<Queue> queues = await db.GetQueues(idRoom);
        Room? room = await db.GetRoom(idRoom);
        if (room != null)
        {
            var text = "Список ваших очередей в комате \"" + room.Name;

            await botClient.SendTextMessageAsync(idChat, text);

            for (int i = 0; i < queues.Count; i++)
            {
                InlineKeyboardButton inlineKeyboardFreez;
                if (queues[i].Freezing)
                {
                    inlineKeyboardFreez = InlineKeyboardButton.WithCallbackData("Разморозить", "R8!" + queues[i].Id.ToString());
                }
                else
                {
                    inlineKeyboardFreez = InlineKeyboardButton.WithCallbackData("Заморозить", "R8!" + queues[i].Id.ToString());
                }
                var ikm = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Добавить окно", "R6!" + queues[i].Id.ToString()),
                        InlineKeyboardButton.WithCallbackData("Удалить", "R7!" + queues[i].Id.ToString()),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Посмотреть окна", "R9!" + queues[i].Id.ToString()),
                        inlineKeyboardFreez
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

    public async Task FreezAllQueueInRoom(ITelegramBotClient botClient, long idChat, string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        Room? room = await db.GetRoom(idRoom);
        if (room != null)
        {
            foreach (Queue queue in room.Queues)
            {
                await db.Freez(queue.Id);
            }
            await botClient.SendTextMessageAsync(idChat, $"Всё заморожено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public async Task AddWindow1(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoomS)
    {
        var ikm = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Понедельник", "R10!1"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Вторник", "R10!2"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Среда", "R10!3"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Четверг", "R10!4"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Пятница", "R10!5"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Суббота", "R10!6"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Воскресенье", "R10!7"),
            },
        });
        await db.ChangeThemeDataOrganization(idOrganization, idRoomS);
        await botClient.SendTextMessageAsync(idChat, "Выберете день недели", replyMarkup: ikm);
    }

    public async Task AddWindow2(ITelegramBotClient botClient, long idChat, long idOrganization, string idWeekDay)
    {
        await db.ChangeThemeOrganization(idOrganization, 3);
        await botClient.SendTextMessageAsync(idChat, $"Введите время начала окна");
        await db.AddPartThemeDataOrganization(idOrganization, idWeekDay);
    }

    public async Task AddWindow3(ITelegramBotClient botClient, long idChat, long idOrganization, string startTime)
    {
        await botClient.SendTextMessageAsync(idChat, $"Введите время очончания окна");
        await db.AddPartThemeDataOrganization(idOrganization, startTime);
        await db.ChangeThemeOrganization(idOrganization, 4);
    }

    public async void AddWindow4(ITelegramBotClient botClient, long idChat, long idOrganization, string endTime)
    {
        string[] data = (await db.GetThemeDataOrganization(idOrganization)).Split("!");
        DateTime? start = StringToDateTime(data[2]);
        if (start != null)
        {
            DateTime? end = StringToDateTime(endTime);
            if (end != null)
            {
                if (await db.IsValidTime(long.Parse(data[0]), long.Parse(data[1]), start, end))
                {
                    if (await db.AddWindow(long.Parse(data[0]), long.Parse(data[1]), start.Value, end.Value))
                    {
                        await botClient.SendTextMessageAsync(idChat, $"Окно успешно добавлено");
                        await db.ChangeThemeDataOrganization(idOrganization, "");
                        return;
                    }
                }
            }
        }
        await db.ChangeThemeDataOrganization(idOrganization, "");
        await botClient.SendTextMessageAsync(idChat, $"Окно не добавлено");
    }

    public async Task DeleteQueue(ITelegramBotClient botClient, long idChat, string idQueueS)
    {
        long idQueue = long.Parse(idQueueS);
        if (await db.DeleteQueue(idQueue))
        {
            await botClient.SendTextMessageAsync(idChat, $"Очередь успешно удалена");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так");
        }
    }

    public async Task FreezUnfreez(ITelegramBotClient botClient, long idChat, string idQueueS)
    {
        long idQueue = long.Parse(idQueueS);
        if (await db.FreezUnfreez(idQueue))
        {
            await botClient.SendTextMessageAsync(idChat, $"Действие успешно выполнено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так");
        }
    }

    public async Task AllWindowsInQueue(ITelegramBotClient botClient, long idChat, string idWindowS)
    {
        await botClient.SendTextMessageAsync(idChat, "Окна по неделям");
        for (int i = 0; i < 7; i++)
        {
            List<Window> windows = await db.GetWindows(long.Parse(idWindowS), i + 1);
            WeekDay? weekDay = await db.GetWeek(i + 1);
            if (weekDay != null)
            {
                if (windows.Count != 0)
                {
                    await botClient.SendTextMessageAsync(idChat, "Окна на " + (weekDay).Name);
                    for (int f = 0; f < windows.Count; f++)
                    {
                        var ikm = new InlineKeyboardMarkup(new[]
                        {
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Изменить промежуток", "R11!" + windows[f].Id),
                            },
                            new[]
                            {
                                InlineKeyboardButton.WithCallbackData("Удалить", "R12!" +  windows[f].Id),
                            },
                        });
                        string header = " Промежуток №" + (f + 1) + "; с " + windows[f].Start.ToString("HH:mm tt") + " - " + windows[f].End.ToString("HH:mm tt") + ";";
                        await botClient.SendTextMessageAsync(idChat, header, replyMarkup: ikm);
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(idChat, "Окна на " + (weekDay).Name + " - Пусто");
                }
            }
        }
    }

    public async Task ChangeTime1(ITelegramBotClient botClient, long idChat, long idOrganization, string idWindow)
    {
        await botClient.SendTextMessageAsync(idChat, "Введите новое начало окна");
        await db.ChangeThemeDataOrganization(idOrganization, idWindow);
        await db.ChangeThemeOrganization(idOrganization, 5);
    }

    public async Task ChangeTime2(ITelegramBotClient botClient, long idChat, long idOrganization, string startTime)
    {
        await botClient.SendTextMessageAsync(idChat, $"Введите время очончания окна");
        await db.AddPartThemeDataOrganization(idOrganization, startTime);
        await db.ChangeThemeOrganization(idOrganization, 6);
    }

    public async Task ChangeTime3(ITelegramBotClient botClient, long idChat, long idOrganization, string endTime)
    {
        string[] data = (await db.GetThemeDataOrganization(idOrganization)).Split("!");
        DateTime? start = StringToDateTime(data[1]);
        if (start != null)
        {
            DateTime? end = StringToDateTime(endTime);
            if (end != null)
            {
                if (await db.IsValidTime(long.Parse(data[0]), start, end))
                {
                    Window? window = await db.GetWindow(long.Parse(data[0]));
                    if (window != null)
                    {
                        if (await db.DeleteWindow(long.Parse(data[0])) && await db.AddWindow(window.QueueId, window.WeekDayId, start.Value, end.Value))
                        {
                            await botClient.SendTextMessageAsync(idChat, $"Промежуток успешно изменён");
                            return;
                        }
                    }
                }
            }
        }
        await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так!");
    }

    static DateTime? StringToDateTime(string date)//format string 14:45
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

    public async Task DeleteWindow(ITelegramBotClient botClient, long idChat, string idWindow)
    {
        if (await db.DeleteWindow(long.Parse(idWindow)))
        {
            await botClient.SendTextMessageAsync(idChat, $"Окно успешно удалено");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так");
        }
    }
}
