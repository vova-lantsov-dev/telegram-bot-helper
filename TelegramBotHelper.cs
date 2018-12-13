using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Helper.Actions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Helper
{
    public class TelegramBotHelper
    {
        private static string _separator = "~";
        /// <summary>
        /// Set callback data separator. Default is "~"
        /// </summary>
        public static string Separator
        {
            get => _separator;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                if (value == string.Empty)
                    throw new ArgumentException("Value cannot be empty string.", "value");

                _separator = value;
            }
        }

        private readonly TelegramBotClient _telegram;

        private readonly List<BotInline> _inlineCallbacks = new List<BotInline>();
        private readonly List<BotTextMessage> _messageCallbacks = new List<BotTextMessage>();
        private readonly List<BotExpression> _expressionCallbacks = new List<BotExpression>();

        /// <summary>
        /// You can create your own update handler using this delegate.
        /// </summary>
        public Func<Update, List<BotInline>, List<BotTextMessage>, List<BotExpression>, Task> CustomUpdateReceived;

        /// <summary>
        /// Verify user on every incoming message. If null, all verify statuses will be set to Unchecked.
        /// </summary>
        public Func<User, Task<Verify>> Verifying;

        /// <summary>
        /// Create new instance of TelegramBotHelper class.
        /// </summary>
        /// <param name="initializer">Initialize TelegramBotClient to use by helper</param>
        /// <param name="separator">Separator which will be used to split callback query data. If null, default is "~".</param>
        public TelegramBotHelper(Func<TelegramBotClient> initializer, string separator = null)
        {
            if (initializer == null)
                throw new ArgumentNullException("initializer");
            if (separator == string.Empty)
                throw new ArgumentException("Separator cannot be empty string", "separator");

            _telegram = initializer();
            if (_telegram == null)
                throw new ArgumentException("Initializer must return not null client", "initializer");

            if (separator != null)
                Separator = separator;
        }

        /// <summary>
        /// Process new telegram update.
        /// </summary>
        /// <param name="update">Incoming update</param>
        public async Task UpdateReceived(Update update)
        {
            if (update == null)
                return;

            if (CustomUpdateReceived != null)
            {
                await CustomUpdateReceived(update, _inlineCallbacks, _messageCallbacks, _expressionCallbacks);
                return;
            }

            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        var message = update.Message;
                        if (message == null)
                            return;
                        var v = Verifying != null
                            ? await Verifying(message.From)
                            : Verify.Unchecked;
                        switch (message.Type)
                        {
                            case MessageType.Text:
                                {
                                    var mCb = _messageCallbacks.Count > 0
                                        ? _messageCallbacks.Find(it => it.Message == message.Text)
                                        : default;
                                    if (mCb != default)
                                    {
                                        if (mCb.Verified == Verify.Unchecked || mCb.Verified.HasFlag(v))
                                            await mCb.Callback(message, v);
                                        break;
                                    }

                                    goto default;
                                }
                            default:
                                {
                                    foreach (var expressionCallback in _expressionCallbacks)
                                    {
                                        if (!await expressionCallback.Expression(message))
                                            continue;

                                        if (expressionCallback.Verified != Verify.Unchecked
                                            && !expressionCallback.Verified.HasFlag(v))
                                            continue;

                                        await expressionCallback.Callback(message, v);
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case UpdateType.CallbackQuery:
                    {
                        var q = update.CallbackQuery;
                        var v = Verifying != null
                            ? await Verifying(q.From)
                            : Verify.Unchecked;

                        var c = new InlineCommand(q.Data, Separator);
                        foreach (var inlineCallback in _inlineCallbacks)
                        {
                            if (!c.Equals(inlineCallback.Command))
                                continue;

                            if (inlineCallback.Verified == Verify.Unchecked || inlineCallback.Verified.HasFlag(v))
                            {
                                await inlineCallback.Callback(new CallbackQueryInfo(q), c.Commands, v);
                                break;
                            }
                        }
                        break;
                    }
            }
        }

        public void Inlines(Action<BotInline.Builder> builder)
        {
            builder(new BotInline.Builder(_inlineCallbacks));
        }
        public void Messages(Action<BotTextMessage.Builder> builder)
        {
            builder(new BotTextMessage.Builder(_messageCallbacks));
        }
        public void Expressions(Action<BotExpression.Builder> builder)
        {
            builder(new BotExpression.Builder(_expressionCallbacks));
        }
    }
}
