using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Helper.Keyboards
{
    /// <summary>
    /// Telegram keyboard builder
    /// </summary>
    public class Builder<T> where T : IKeyboardButton
    {
        internal readonly List<List<T>> Keyboard = new List<List<T>>
        {
            new List<T>()
        };

        internal int Index = 0;
    }
}
