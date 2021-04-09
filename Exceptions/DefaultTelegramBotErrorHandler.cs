using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Helper.Commands;

namespace Telegram.Bot.Helper.Exceptions
{
    /// <inheritdoc />
    public class DefaultTelegramBotErrorHandler : ITelegramBotErrorHandler
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultTelegramBotErrorHandler"/> class.
        /// </summary>
        /// <param name="loggerFactory">A logger factory used to create the loggers for commands or network errors.</param>
        public DefaultTelegramBotErrorHandler(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <inheritdoc />
        public virtual ValueTask HandleNetworkErrorAsync(ITelegramBotClient client, Exception e, CancellationToken ct)
        {
            var logger = _loggerFactory.CreateLogger("TelegramBotUpdateHandler");
            logger.LogError(e, "Network error occurred while handling telegram bot updates.");
            return new ValueTask();
        }

        /// <inheritdoc />
        public virtual ValueTask HandleCommandErrorAsync(ITelegramBotClient client, TelegramCommand command, Exception e,
            CancellationToken ct)
        {
            var logger = _loggerFactory.CreateLogger(command.GetType());
            logger.LogError(e, "Error occurred while handling telegram bot command.");
            return new ValueTask();
        }
    }
}