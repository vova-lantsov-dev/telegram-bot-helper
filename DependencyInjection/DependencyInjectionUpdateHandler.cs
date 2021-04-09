using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Helper.Exceptions;
using Telegram.Bot.Helper.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Telegram.Bot.Helper.DependencyInjection
{
    internal sealed class DependencyInjectionUpdateHandler : IUpdateHandler
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ITelegramBotErrorHandler _errorHandler;
        private readonly ITelegramBotCommandManager _commandManager;
        private readonly TelegramBotOptions _options;

        public DependencyInjectionUpdateHandler(
            IOptions<TelegramBotOptions> options,
            ILoggerFactory loggerFactory,
            ITelegramBotErrorHandler errorHandler,
            ITelegramBotCommandManager commandManager)
        {
            _loggerFactory = loggerFactory;
            _errorHandler = errorHandler;
            _commandManager = commandManager;
            _options = options.Value;
        }

        public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var command = await _commandManager.GetCommandForUpdateAsync(update, cancellationToken);
            if (command == null)
                return;

            using (command.ServiceScope)
            {
                await command.HandleUpdateAsync();
            }
        }

        public async Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            await _errorHandler.HandleNetworkErrorAsync(botClient, exception, cancellationToken);
        }

        public UpdateType[] AllowedUpdates =>
            _options.AllowedUpdates?.Select(it => (UpdateType) Enum.Parse(typeof(UpdateType), it, true)).ToArray();
    }
}