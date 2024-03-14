using DataBase.Context;
using DataBase.Models;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace Client;

public static class ClientInteractionDB
{
    public static async Task Theme0(long idOrganization)
    {
        await MainDataBase.ChangeThemeOrganization(idOrganization, 0);
    }

    public static async Task<int> GetTheme(long idOrganization)
    {
        return await MainDataBase.GetThemeClient(idOrganization);
    }

    public static async Task SingUp(ITelegramBotClient botClient, long idChat, long idClient)
    {
        if (await ClientDataBase.AddClient(idClient))
        {
            await botClient.SendTextMessageAsync(idChat, "Вы были успешно зарегистрированы!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Вы уже зарегистрированы :(");
        }
    }

    public static async Task Search1(ITelegramBotClient botClient, long idChat, long idClient)
    {
        await botClient.SendTextMessageAsync(idChat,
            "Хорошо. Пожалуйста, используйте этот формат (1-по имени/2-по идентификатору):\r\n\r\nПример \u21161: 1:Название организации\r\nПример \u21162: 2:456848\r\n");
        await MainDataBase.ChangeThemeClient(idClient, 1);
    }

    public static async Task AddReserveds(ITelegramBotClient botClient, long idChat, long idClient, long idWindow,
        DateTime dateTime)
    {
        if (await ReservedDataBase.AddReserved(idClient, idWindow, dateTime))
        {
            await botClient.SendTextMessageAsync(idChat, "Окно успешно зарезервировано!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public static async Task GetReserved(ITelegramBotClient botClient, long idChat, long idClient)
    {
        List<Reserved> reserveds = await ClientDataBase.GetReserved(idClient);
        if (reserveds.Count > 0)
        {
            InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[reserveds.Count][];

            for (int i = 0; i < reserveds.Count; i++)
            {
                InlineKeyboardButton[] inlineKeyboardButtons1 =
                [
                    InlineKeyboardButton.WithCallbackData(
                        "Убрать запись " + reserveds[i].Date.ToShortDateString(),
                        "DeleteRecord!" + reserveds[i].WindowId)
                ];
                inlineKeyboardButtons[i] = [inlineKeyboardButtons1[0]];
            }

            InlineKeyboardMarkup ikm = new(inlineKeyboardButtons);
            await botClient.SendTextMessageAsync(idChat, "Записи", replyMarkup: ikm);
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Список зарезервированных окон - пуст");
        }
    }

    public static async Task AllWindowInQueue1(ITelegramBotClient botClient, long idChat, long idWindow)
    {
        Window? window = await WindowDataBase.GetWindow(idWindow);
        if (window != null)
        {
            DateTime nextMonth = DateTime.Today.AddMonths(1);
            DateTime startDate = DateTime.Today;
            while ((int)startDate.DayOfWeek != window.WeekDayId)
            {
                startDate = startDate.AddDays(1);
            }

            List<DateTime> dateTimes = [];
            while (startDate < nextMonth)
            {
                dateTimes.Add(startDate);
                startDate = startDate.AddDays(7);
            }

            List<Reserved> reserveds = await WindowDataBase.GetReserved(idWindow);
            foreach (Reserved reserved in reserveds)
            {
                foreach (DateTime dateTime in dateTimes)
                {
                    if (reserved.Date == dateTime)
                    {
                        dateTimes.Remove(dateTime);
                        break;
                    }
                }
            }

            if (dateTimes.Count > 0)
            {
                InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[dateTimes.Count][];

                for (int i = 0; i < dateTimes.Count; i++)
                {
                    InlineKeyboardButton[] inlineKeyboardButtons1 =
                    [
                        InlineKeyboardButton.WithCallbackData(dateTimes[i].ToShortDateString(),
                            "AddReserved!" + idWindow + "#" + dateTimes[i].ToShortDateString())
                    ];
                    inlineKeyboardButtons[i] = [inlineKeyboardButtons1[0]];
                }

                InlineKeyboardMarkup ikm = new(inlineKeyboardButtons);
                await botClient.SendTextMessageAsync(idChat, "Даты доступные для записи", replyMarkup: ikm);
            }
            else
            {
                await botClient.SendTextMessageAsync(idChat, "Доступных дат - нет");
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public static async Task AllWindowInQueue(ITelegramBotClient botClient, long idChat, string idQueueS)
    {
        await botClient.SendTextMessageAsync(idChat, "Окна по неделям");
        for (int i = 0; i < 7; i++)
        {
            List<Window> windows = await WindowDataBase.GetWindows(long.Parse(idQueueS), i + 1);
            InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[windows.Count][];

            for (int f = 0; f < windows.Count; f++)
            {
                string header = "Забронировать окно c " + windows[f].Start.ToString("HH:mm tt") + " - " +
                                windows[f].End.ToString("HH:mm tt");
                InlineKeyboardButton[] inlineKeyboardButtons1 =
                [
                    InlineKeyboardButton.WithCallbackData(header, "BookWindow!" + windows[f].Id)
                ];
                inlineKeyboardButtons[f] = [inlineKeyboardButtons1[0]];
            }

            InlineKeyboardMarkup ikm = new(inlineKeyboardButtons);
            WeekDay? weekDay = await WindowDataBase.GetWeek(i + 1);
            if (weekDay != null)
            {
                if (windows.Count == 0)
                {
                    await botClient.SendTextMessageAsync(idChat, "Окон на " + weekDay.Name + " - нет");
                }
                else
                {
                    await botClient.SendTextMessageAsync(idChat, "Окна на " + weekDay.Name, replyMarkup: ikm);
                }
            }
        }
    }

    public static async Task AllQueueInRoom(ITelegramBotClient botClient, long idChat, long idOrganization,
        string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        await MainDataBase.ChangeThemeOrganization(idOrganization, 3);
        Room? room = await RoomDataBase.GetRoom(idRoom);
        List<Queue> queues = await QueueDataBase.GetQueues(idRoom);
        if (room != null)
        {
            string text = "Список ваших очередей в комате " + room.Name;
            InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[queues.Count][];

            for (int i = 0; i < queues.Count; i++)
            {
                InlineKeyboardButton[] inlineKeyboardButtons1 =
                [
                    InlineKeyboardButton.WithCallbackData("Посмотреть очередь № " + (i + 1),
                        "ViewQueue!" + queues[i].Id)
                ];
                inlineKeyboardButtons[i] = [inlineKeyboardButtons1[0]];
            }

            InlineKeyboardMarkup ikm = new(inlineKeyboardButtons);
            await botClient.SendTextMessageAsync(idChat, text, replyMarkup: ikm);
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public static async Task Search2(ITelegramBotClient botClient, long idChat, long idClient, string data)
    {
        string[] parameters = data.Split(":");
        if (parameters.Length == 2)
        {
            if (parameters[0] == "1")
            {
                await AllRooms(botClient, idChat, idClient, await OrganizationDataBase.SearchNameOrg(parameters[1]));
            }
            else if (parameters[0] == "2")
            {
                if (long.TryParse(parameters[1], out long id))
                {
                    await AllRooms(botClient, idChat, idClient, await OrganizationDataBase.SearchIdOrg(id));
                }
                else
                {
                    await botClient.SendTextMessageAsync(idChat, "Некорректный ввод");
                }
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Некорректный ввод");
        }
    }

    public static async Task AddFavorites(ITelegramBotClient botClient, long idChat, long idClient, long idRoom)
    {
        if (await RoomDataBase.AddFavorite(idClient, idRoom))
        {
            await botClient.SendTextMessageAsync(idChat, "Комната успешно добавлена в избранное!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public static async Task GetFavorites(ITelegramBotClient botClient, long idChat, long idClient)
    {
        List<Room> rooms = await RoomDataBase.GetFavoritesRoom(idClient);
        if (rooms.Count > 0)
        {
            await AllRooms(botClient, idChat, idClient, rooms);
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Список избранных комнат - пуст");
        }
    }

    public static async Task DeleteFavorites(ITelegramBotClient botClient, long idChat, long idClient, long idRoom)
    {
        if (await RoomDataBase.DeleteFavorite(idClient, idRoom))
        {
            await botClient.SendTextMessageAsync(idChat, "Комната успешно удалена из избранного!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }

    public static async Task DeleteReserved(ITelegramBotClient botClient, long idChat, long idClient, long idWindow)
    {
        if (await ReservedDataBase.DeleteReserved(idClient, idWindow))
        {
            await botClient.SendTextMessageAsync(idChat, "Окно успешно разрезервировано!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }


    static async Task AllRooms(ITelegramBotClient botClient, long idChat, long idClient, List<Room> rooms)
    {
        string text = "Список комнат!";

        await botClient.SendTextMessageAsync(
            idChat,
            text);

        for (int i = 0; i < rooms.Count; i++)
        {
            Organization? organization = rooms[i].Organization;
            if (organization != null)
            {
                InlineKeyboardButton inlineKeyboardButton;
                if (rooms[i].Favorites == null || (rooms[i].Favorites != null &&
                                                   !rooms[i].Favorites.Any(p =>
                                                       p.ClientId == idClient && p.RoomId == rooms[i].Id)))
                {
                    inlineKeyboardButton =
                        InlineKeyboardButton.WithCallbackData("Добавить в избранное", "AddToFavorites!" + rooms[i].Id);
                }
                else
                {
                    inlineKeyboardButton = InlineKeyboardButton.WithCallbackData("Удалить из избранного",
                        "RemoveFromFavorites!" + rooms[i].Id);
                }

                InlineKeyboardMarkup ikm = new(new[]
                {
                    [
                        InlineKeyboardButton.WithCallbackData("Посмотреть очереди", "ViewQueues!" + rooms[i].Id)
                    ],
                    new[]
                    {
                        inlineKeyboardButton
                    }
                });
                string header = "№" + (i + 1) + " Название организации: " + organization.Name + "; Название комнаты: " +
                                rooms[i].Name + "; Количесвто очередей: ";
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
            else
            {
                await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
            }
        }
    }
}