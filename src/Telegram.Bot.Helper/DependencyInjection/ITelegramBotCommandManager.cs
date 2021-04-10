using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Helper.Commands;
using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.DependencyInjection
{
    internal interface ITelegramBotCommandManager
    {
        ValueTask<TelegramCommand> GetCommandForUpdateAsync(Update update, CancellationToken ct);
    }
}