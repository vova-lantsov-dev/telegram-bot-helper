using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telegram.Bot.Helper.Actions
{
    public class BotInline
    {
        public readonly InlineCommand Command;
        public readonly Func<CallbackQueryInfo, string[], Verify, Task> Callback;
        public readonly Verify Verified;

        public BotInline(Func<CallbackQueryInfo, string[], Verify, Task> callback, string command, string separator, Verify verified)
        {
            Command = new InlineCommand(command, separator);
            Callback = callback;
            Verified = verified;
        }

        public class Builder
        {
            public readonly List<BotInline> Callbacks;
            internal Builder(List<BotInline> callbacks)
            {
                Callbacks = callbacks;
            }

            public Func<CallbackQueryInfo, string[], Verify, Task> this[string command, Verify verified = Verify.Unchecked]
            {
                set => Callbacks.Add(new BotInline(value, command, TelegramBotHelper.Separator, verified));
            }
        }
    }
}
