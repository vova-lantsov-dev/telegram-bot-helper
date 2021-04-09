using System;
using System.Threading;
using System.Threading.Tasks;

namespace Telegram.Bot.Helper.Options
{
    /// <summary>
    /// Custom options for exception handling of telegram bot requests.
    /// </summary>
    public sealed class TelegramBotErrorOptions
    {
        public Func<ITelegramBotClient, Exception, CancellationToken, Task> CustomNetworkExceptionHandling { get; set; }
    }
}