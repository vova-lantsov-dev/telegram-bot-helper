using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.Commands
{
    public abstract class TelegramCommand
    {
        public Update Update { get; internal set; }

        public virtual bool CanHandle() => true;

        public virtual ValueTask<bool> CanHandleAsync() => new ValueTask<bool>(true);

        public CancellationToken CancellationToken { get; internal set; }
        
        internal IServiceScope ServiceScope { get; set; }

        protected internal abstract Task HandleUpdateAsync();
    }
}