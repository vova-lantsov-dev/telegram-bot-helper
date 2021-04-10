using System;

namespace Telegram.Bot.Helper.Attributes
{
    public abstract class TelegramBotFilterAttribute : Attribute
    {
        protected internal abstract bool IsValid();
    }
}