using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Helper.Keyboards
{
    public static class InlineKeyboardBuilderExtensions
    {
        #region Build

        /// <summary>
        /// Add inline button to the keyboard.
        /// </summary>
        /// <param name="text">Button's text. Not null.</param>
        /// <param name="command">Callback data for this button. Can be null (url needed).</param>
        /// <param name="url">Url to navigate to after click</param>
        public static Builder<InlineKeyboardButton> B(this Builder<InlineKeyboardButton> builder, string text, string command, string url = null)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (text == null)
                throw new ArgumentNullException("text");

            builder.Keyboard[builder.Index].Add(new InlineKeyboardButton { CallbackData = command, Text = text, Url = url });
            return builder;
        }

        /// <summary>
        /// Add inline button to the keyboard.
        /// </summary>
        public static Builder<InlineKeyboardButton> B(this Builder<InlineKeyboardButton> builder, InlineKeyboardButton button)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (button == null)
                throw new ArgumentNullException("button");

            builder.Keyboard[builder.Index].Add(button);
            return builder;
        }

        #endregion

        #region Markup

        /// <summary>
        /// Build markup
        /// </summary>
        public static InlineKeyboardMarkup M(this IEnumerable<IEnumerable<InlineKeyboardButton>> keyboard) =>
            new InlineKeyboardMarkup(keyboard);

        /// <summary>
        /// Build markup
        /// </summary>
        public static InlineKeyboardMarkup M(this IEnumerable<InlineKeyboardButton> keyboard) =>
            new InlineKeyboardMarkup(keyboard);

        /// <summary>
        /// Build markup
        /// </summary>
        public static InlineKeyboardMarkup M(this InlineKeyboardButton keyboard) =>
            new InlineKeyboardMarkup(keyboard);

        /// <summary>
        /// Build markup
        /// </summary>
        public static InlineKeyboardMarkup M(this Builder<InlineKeyboardButton> builder) =>
            new InlineKeyboardMarkup(builder.Keyboard);

        #endregion
    }
}
