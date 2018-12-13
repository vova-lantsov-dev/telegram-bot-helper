using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Helper.Keyboards
{
    public static class KeyboardBuilderExtensions
    {
        /// <summary>
        /// Add new line to keyboard
        /// </summary>
        public static Builder<TButton> L<TButton>(this Builder<TButton> builder) where TButton : IKeyboardButton
        {
            ++builder.Index;
            builder.Keyboard.Add(new List<TButton>());
            return builder;
        }
    }
}
