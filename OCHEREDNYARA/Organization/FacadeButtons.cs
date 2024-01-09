using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot;

public class FacadeButtons
{
    static ReplyKeyboardMarkup ToReplyKeyboardMarkup(List<KeyboardButton[]> keyboard)
    {
        return new ReplyKeyboardMarkup(keyboard)
        {
            ResizeKeyboard = true,
        };
    }

    public static ReplyKeyboardMarkup MainOrganization()
    {
        return ToReplyKeyboardMarkup(
        new List<KeyboardButton[]>()
        {
                new KeyboardButton[]
                {
                    new KeyboardButton("Личный кабинет"),
                    new KeyboardButton("Создать комнату"),
                    new KeyboardButton("Посмотреть список комнат"),
                },
        });
    }


    public static ReplyKeyboardMarkup Empty()
    {
        return ToReplyKeyboardMarkup(
        new List<KeyboardButton[]>()
        {

        });
    }
}
