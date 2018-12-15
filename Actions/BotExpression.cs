using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.Actions
{
    public class BotExpression
    {
        public readonly Func<Message, bool> Expression;
        public readonly Func<Message, Verify, Task> Callback;
        public readonly Verify Verified;

        public BotExpression(Func<Message, bool> expression,
            Func<Message, Verify, Task> callback,
            Verify verified)
        {
            Callback = callback;
            Expression = expression;
            Verified = verified;
        }

        public class Builder
        {
            public readonly List<BotExpression> _expressionList;
            public Builder(ref List<BotExpression> expressionList) => _expressionList = expressionList;
            
            public Func<Message, Verify, Task> this[Func<Message, bool> expression,
                Verify verified = Verify.Unchecked]
            {
                set => _expressionList.Add(new BotExpression(expression, value, verified));
            }
        }
    }
}
