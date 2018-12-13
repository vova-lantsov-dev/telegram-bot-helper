using System;
using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Helper.Keyboards
{
    public static class ReplyKeyboardMarkupExtensions
    {
        #region Build

        /// <summary>
        /// Add button to the keyboard.
        /// </summary>
        /// <param name="text">Button's text. Not null.</param>
        /// <param name="requestContact">True if button should send user's contact on click</param>
        /// <param name="requestLocation">True if button should send user's current location on click</param>
        public static Builder<KeyboardButton> B(this Builder<KeyboardButton> builder, string text, bool requestContact = false, bool requestLocation = false)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (text == null)
                throw new ArgumentNullException("text");

            builder.Keyboard[builder.Index].Add(new KeyboardButton { Text = text, RequestContact = requestContact, RequestLocation = requestLocation });
            return builder;
        }

        /// <summary>
        /// Add button to the keyboard.
        /// </summary>
        public static Builder<KeyboardButton> B(this Builder<KeyboardButton> builder, KeyboardButton button)
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
        public static ReplyKeyboardMarkup M(this IEnumerable<IEnumerable<KeyboardButton>> keyboard) =>
            new ReplyKeyboardMarkup(keyboard);

        /// <summary>
        /// Build markup
        /// </summary>
        public static ReplyKeyboardMarkup M(this IEnumerable<KeyboardButton> keyboard) =>
            new ReplyKeyboardMarkup(keyboard);

        /// <summary>
        /// Builds markup
        /// </summary>
        public static ReplyKeyboardMarkup M(this KeyboardButton keyboard) =>
            new ReplyKeyboardMarkup(keyboard);

        /// <summary>
        /// Build markup
        /// </summary>
        public static ReplyKeyboardMarkup M(this Builder<KeyboardButton> builder) =>
            new ReplyKeyboardMarkup(builder.Keyboard);

        /// <summary>
        /// Set ResizeKeyboard property value
        /// </summary>
        /// <param name="value">New value</param>
        public static ReplyKeyboardMarkup RK(this ReplyKeyboardMarkup markup, bool value = true)
        {
            markup.ResizeKeyboard = value;
            return markup;
        }

        /// <summary>
        /// Set OneTimeKeyboard property value
        /// </summary>
        /// <param name="value">New value</param>
        public static ReplyKeyboardMarkup OTK(this ReplyKeyboardMarkup markup, bool value = true)
        {
            markup.OneTimeKeyboard = value;
            return markup;
        }

        #endregion
    }
}
