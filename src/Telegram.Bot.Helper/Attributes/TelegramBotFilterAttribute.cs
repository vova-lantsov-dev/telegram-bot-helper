using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class TelegramBotFilterAttribute : Attribute
    {
        public Update Update { get; internal set; }
        
        public virtual Type[] ParentFilters { get; }
        
        protected internal virtual bool IsValid() => true;

        protected internal virtual ValueTask<bool> IsValidAsync() => new ValueTask<bool>(true);

        internal async ValueTask<bool> ValidateChildrenAsync()
        {
            var type = GetType();
            var filtersAsAttribute = type.GetCustomAttributes(type, true);
            var parentAttributes = ParentFilters ?? Array.Empty<Type>();
            var allFilters = filtersAsAttribute.Union(parentAttributes);
            return true;
        }
    }
}