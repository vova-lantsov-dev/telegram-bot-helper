using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Helper.Commands;

namespace Telegram.Bot.Helper.Exceptions
{
    /// <summary>
    /// Telegram bot error handling abstraction.
    /// </summary>
    public interface ITelegramBotErrorHandler
    {
        /// <summary>
        /// Handles all network-related errors (such as when internet connection is lost or Telegram servers are offline).
        /// </summary>
        /// <param name="client">An instance of telegram bot client that can be used to send the requests to the Telegram servers.</param>
        /// <param name="e">An exception that you should log somehow.</param>
        /// <param name="ct">A cancellation token that can be triggered by .NET host shutdown.</param>
        /// <returns></returns>
        ValueTask HandleNetworkErrorAsync(ITelegramBotClient client, Exception e, CancellationToken ct);

        /// <summary>
        /// Handles all errors thrown in any command.
        /// </summary>
        /// <param name="client">An instance of telegram bot client that can be used to send the requests to the Telegram servers.</param>
        /// <param name="command">A command abstraction for all kinds of command (message, callback query, etc.)</param>
        /// <param name="e">An exception that you should log somehow.</param>
        /// <param name="ct">A cancellation token that can be triggered by .NET host shutdown.</param>
        /// <returns></returns>
        ValueTask HandleCommandErrorAsync(ITelegramBotClient client, TelegramCommand command,
            Exception e, CancellationToken ct);
    }
}