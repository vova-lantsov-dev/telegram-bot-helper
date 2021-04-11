using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.Attributes
{
    public abstract class TelegramBotFilterAttribute : Attribute
    {
        public Update Update { get; internal set; }
        
        public virtual TelegramBotFilterAttribute[] ParentFilters { get; }
        
        protected internal virtual bool IsValid() => true;

        protected internal virtual ValueTask<bool> IsValidAsync() => new ValueTask<bool>(true);
    }
}