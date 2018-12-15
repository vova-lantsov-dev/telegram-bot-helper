using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.Actions
{
    public class BotTextMessage
    {
        public readonly string Message;
        public readonly Func<Message, Verify, Task> Callback;
        public readonly Verify Verified;

        public BotTextMessage(Func<Message, Verify, Task> callback, string message, Verify verified)
        {
            Message = message;
            Callback = callback;
            Verified = verified;
        }

        public class Builder
        {
            public readonly List<BotTextMessage> _messages;
            internal Builder(ref List<BotTextMessage> messages) => _messages = messages;
            
            public Func<Message, Verify, Task> this[string message, Verify verified = Verify.Unchecked]
            {
                set
                {
                    _messages.Add(new BotTextMessage(value, message, verified));
                }
            }

            public Func<Message, Verify, Task> this[string[] messages, Verify verified = Verify.Unchecked]
            {
                set
                {
                    foreach (var message in messages)
                        _messages.Add(new BotTextMessage(value, message, verified));
                }
            }
        }
    }
}
