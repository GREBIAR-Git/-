using DataBase.Context;
using DataBase.Models;
using System.Data;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

class Client
{
    readonly ContextDataBase db;

    public Client()
    {
        db = new();
    }

    public async Task Theme0(long idOrganization)
    {
        await db.ChangeThemeOrganization(idOrganization, 0);
    }

    public async Task<int> GetTheme(long idOrganization)
    {
        return await db.GetThemeClient(idOrganization);
    }

    public async Task SingUp(ITelegramBotClient botClient, long idChat, long idClient)
    {
        if (await db.AddClient(idClient))
        {
            await botClient.SendTextMessageAsync(idChat, $"Вы были успешно зарегистрированы!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Вы уже зарегистрированы :(");
        }
    }

    public async Task Search1(ITelegramBotClient botClient, long idChat, long idClient)
    {
        await botClient.SendTextMessageAsync(idChat, $"Хорошо. Пожалуйста, используйте этот формат (1-по имени/2-по идентификатору):\r\n\r\nПример №1: 1:Название организации\r\nПример №2: 2:456848\r\n");
        await db.ChangeThemeClient(idClient, 1);
    }
    public async Task AddReserveds(ITelegramBotClient botClient, long idChat, long idClient, long idWindow, DateTime dateTime)
    {
        if (await db.AddReserved(idClient, idWindow, dateTime))
        {
            await botClient.SendTextMessageAsync(idChat, $"Окно успешно зарезервировано!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так!");
        }
    }

    public async Task GetReserved(ITelegramBotClient botClient, long idChat, long idClient)
    {
        List<Reserved> reserveds = await db.GetReserved(idClient);
        if (reserveds.Count > 0)
        {
            InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[reserveds.Count][];

            for (int i = 0; i < reserveds.Count; i++)
            {
                InlineKeyboardButton[] inlineKeyboardButtons1 = new InlineKeyboardButton[1];
                inlineKeyboardButtons1[0] = InlineKeyboardButton.WithCallbackData("Убрать запись " + reserveds[i].Date.ToShortDateString(), "DeleteRecord!" + reserveds[i].WindowId);
                inlineKeyboardButtons[i] = new[] { inlineKeyboardButtons1[0] };
            }
            var ikm = new InlineKeyboardMarkup(inlineKeyboardButtons);
            await botClient.SendTextMessageAsync(idChat, "Записи", replyMarkup: ikm);
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Список зарезервированных окон - пуст");
        }
    }

    public async Task AllWindowInQueue1(ITelegramBotClient botClient, long idChat, long idWindow)
    {

        Window? window = await db.GetWindow(idWindow);
        if (window != null)
        {
            DateTime nextMonth = DateTime.Today.AddMonths(1);
            DateTime startDate = DateTime.Today;
            while ((int)startDate.DayOfWeek != window.WeekDayId)
            {
                startDate = startDate.AddDays(1);
            }
            List<DateTime> dateTimes = new();
            while (startDate < nextMonth)
            {
                dateTimes.Add(startDate);
                startDate = startDate.AddDays(7);
            }
            List<Reserved> reserveds = await db.GetReserveds(idWindow);
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
                    InlineKeyboardButton[] inlineKeyboardButtons1 = new InlineKeyboardButton[1];
                    inlineKeyboardButtons1[0] = InlineKeyboardButton.WithCallbackData(dateTimes[i].ToShortDateString(), "AddReserved!" + idWindow + "#" + dateTimes[i].ToShortDateString());
                    inlineKeyboardButtons[i] = new[] { inlineKeyboardButtons1[0] };
                }
                var ikm = new InlineKeyboardMarkup(inlineKeyboardButtons);
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

    public async Task AllWindowInQueue(ITelegramBotClient botClient, long idChat, string idQueueS)
    {
        await botClient.SendTextMessageAsync(idChat, "Окна по неделям");
        for (int i = 0; i < 7; i++)
        {
            List<Window> windows = await db.GetWindows(long.Parse(idQueueS), i + 1);
            InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[windows.Count][];

            for (int f = 0; f < windows.Count; f++)
            {

                string header = "Забронировать окно c " + windows[f].Start.ToString("HH:mm tt") + " - " + windows[f].End.ToString("HH:mm tt");
                InlineKeyboardButton[] inlineKeyboardButtons1 = new InlineKeyboardButton[1];
                inlineKeyboardButtons1[0] = InlineKeyboardButton.WithCallbackData(header, "BookWindow!" + windows[f].Id);
                inlineKeyboardButtons[f] = new[] { inlineKeyboardButtons1[0] };

            }
            var ikm = new InlineKeyboardMarkup(inlineKeyboardButtons);
            WeekDay? weekDay = await db.GetWeek(i + 1);
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

    public async Task AllQueueInRoom(ITelegramBotClient botClient, long idChat, long idOrganization, string idRoomS)
    {
        long idRoom = long.Parse(idRoomS);
        await db.ChangeThemeOrganization(idOrganization, 3);
        Room? room = await db.GetRoom(idRoom);
        List<Queue> queues = await db.GetQueues(idRoom);
        if (room != null)
        {
            var text = "Список ваших очередей в комате " + room.Name;
            InlineKeyboardButton[][] inlineKeyboardButtons = new InlineKeyboardButton[queues.Count][];

            for (int i = 0; i < queues.Count; i++)
            {
                InlineKeyboardButton[] inlineKeyboardButtons1 = new InlineKeyboardButton[1];
                inlineKeyboardButtons1[0] = InlineKeyboardButton.WithCallbackData("Посмотреть очередь № " + (i + 1), "ViewQueue!" + queues[i].Id.ToString());
                inlineKeyboardButtons[i] = new[] { inlineKeyboardButtons1[0] };
            }
            var ikm = new InlineKeyboardMarkup(inlineKeyboardButtons);
            await botClient.SendTextMessageAsync(idChat, text, replyMarkup: ikm);
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, "Что-то пошло не так!");
        }
    }
    public async Task Search2(ITelegramBotClient botClient, long idChat, long idClient, string data)
    {
        string[] parameters = data.Split(":");
        if (parameters.Length == 2)
        {
            if (parameters[0] == "1")
            {
                await AllRooms(botClient, idChat, idClient, await db.SearchNameOrg(parameters[1]));
            }
            else if (parameters[0] == "2")
            {
                if (long.TryParse(parameters[1], out long id))
                {
                    await AllRooms(botClient, idChat, idClient, await db.SearchIdOrg(id));
                }
                else
                {
                    await botClient.SendTextMessageAsync(idChat, $"Некорректный ввод");
                }
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Некорректный ввод");
        }
    }

    public async Task AddFavorits(ITelegramBotClient botClient, long idChat, long idClient, long idRoom)
    {
        if (await db.AddFavorite(idClient, idRoom))
        {
            await botClient.SendTextMessageAsync(idChat, $"Комната успешно добавлена в избранное!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так!");
        }
    }

    public async Task GetFavorits(ITelegramBotClient botClient, long idChat, long idClient)
    {
        List<Room> rooms = await db.GetFavoritsRoom(idClient);
        if (rooms.Count > 0)
        {
            await AllRooms(botClient, idChat, idClient, rooms);
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Список избранных комнат - пуст");
        }
    }

    public async Task DeleteFavorits(ITelegramBotClient botClient, long idChat, long idClient, long idRoom)
    {
        if (await db.DeleteFavorite(idClient, idRoom))
        {
            await botClient.SendTextMessageAsync(idChat, $"Комната успешно удалена из избранного!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так!");
        }
    }

    public async Task DeleteReserved(ITelegramBotClient botClient, long idChat, long idClient, long idWindow)
    {
        if (await db.DeleteReserved(idClient, idWindow))
        {
            await botClient.SendTextMessageAsync(idChat, $"Окно успешно разрезервировано!");
        }
        else
        {
            await botClient.SendTextMessageAsync(idChat, $"Что-то пошло не так!");
        }
    }


    static async Task AllRooms(ITelegramBotClient botClient, long idChat, long idClient, List<Room> rooms)
    {
        var text = "Список комнат!";

        await botClient.SendTextMessageAsync(
        idChat,
        text);

        InlineKeyboardButton inlineKeyboardButton;

        for (int i = 0; i < rooms.Count; i++)
        {
            Organization? organization = rooms[i].Organization;
            if (organization != null)
            {
                if (rooms[i].Favorites == null || rooms[i].Favorites != null && !rooms[i].Favorites.Where(p => p.ClientId == idClient && p.RoomId == rooms[i].Id).Any())
                {
                    inlineKeyboardButton = InlineKeyboardButton.WithCallbackData("Добавить в избранное", "AddToFavorites!" + rooms[i].Id.ToString());
                }
                else
                {
                    inlineKeyboardButton = InlineKeyboardButton.WithCallbackData("Удалить из избранного", "RemoveFromFavorites!" + rooms[i].Id.ToString());
                }
                var ikm = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Посмотреть очереди", "ViewQueues!" + rooms[i].Id.ToString()),
                    },
                    new[]
                    {
                        inlineKeyboardButton
                    }
                });
                string header = "№" + (i + 1) + " Название организации: " + organization.Name + "; Название комнаты: " + rooms[i].Name + "; Количесвто очередей: ";
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
