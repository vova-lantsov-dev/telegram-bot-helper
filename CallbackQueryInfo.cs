using Telegram.Bot.Types;

namespace Telegram.Bot.Helper
{
    /// <summary>
    /// Helper class to work with callback query
    /// </summary>
    public class CallbackQueryInfo
    {
        /// <summary>
        /// Original callback query object
        /// </summary>
        public readonly CallbackQuery Query;

        /// <summary>
        /// Id of message related to current callback query. Equals to 0 if message is null.
        /// </summary>
        public int MessageId => Query.Message?.MessageId ?? 0;

        /// <summary>
        /// Id of chat related to current callback query. Equals to 0 if message is null.
        /// </summary>
        public long ChatId => Query.Message?.Chat.Id ?? 0L;

        /// <summary>
        /// Id of user related to current callback query.
        /// </summary>
        public int UserId => Query.From.Id;

        /// <summary>
        /// Text of message related to current callback query. Can't be null to prevent NullReferenceException. Its value is empty string if message is null.
        /// </summary>
        public string MessageText => Query.Message?.Text ?? string.Empty;

        internal CallbackQueryInfo(CallbackQuery query) => Query = query;
    }
}
