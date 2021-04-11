using System;
using System.Threading.Tasks;

namespace Telegram.Bot.Helper.Attributes
{
    public abstract class TelegramBotFilterAttribute : Attribute
    {
        protected internal virtual bool IsValid() => true;

        protected internal virtual ValueTask<bool> IsValidAsync() => new ValueTask<bool>(true);
    }
}