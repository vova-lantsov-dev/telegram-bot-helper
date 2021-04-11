using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.Attributes
{
    public abstract class TelegramBotMessageFilterAttribute : TelegramBotFilterAttribute
    {
        public Message Message => Update.Message;

        protected internal override bool IsValid() => Message != null;
    }
}