using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot.Helper.Attributes;
using Telegram.Bot.Helper.Commands;
using Telegram.Bot.Helper.Options;
using Telegram.Bot.Types;

namespace Telegram.Bot.Helper.DependencyInjection
{
    internal sealed class AttributeFilterCommandManager : ITelegramBotCommandManager
    {
        private readonly ReadOnlyCollection<CommandMetadata> _metadataInfo;
        
        internal IServiceProvider ServiceProvider { private get; set; }

        public AttributeFilterCommandManager(
            IOptions<TelegramBotOptions> options)
        {
            string[] assemblies = options.Value.Assemblies;
            assemblies = assemblies == null
                ? new[] {GetType().Assembly.GetName().Name}
                : assemblies.Append(GetType().Assembly.GetName().Name).Distinct().ToArray();

            Assembly[] assemblyTypes = assemblies.Select(Assembly.Load).ToArray();
            
            var metadataInfo = new List<CommandMetadata>();
            foreach (var assembly in assemblyTypes)
            {
                AddCommandMetadataForAssembly(assembly, metadataInfo);
            }
            _metadataInfo = metadataInfo.AsReadOnly();
        }
        
        public async ValueTask<TelegramCommand> GetCommandForUpdateAsync(Update update, CancellationToken ct)
        {
            IServiceScope scope = ServiceProvider.CreateScope();
            foreach (var commandMetadata in _metadataInfo)
            {
                var attributes =
                    commandMetadata.Attributes.Select(att => att.CreateAttributeFunc(ServiceProvider)).ToArray();

                if (!attributes.All(att => att.IsValid()))
                    continue;

                var command = (TelegramCommand) ActivatorUtilities.CreateInstance(scope.ServiceProvider, commandMetadata.CommandType);
                command.Update = update;
                command.CancellationToken = ct;
                command.ServiceScope = scope;
                
                // ReSharper disable once MethodHasAsyncOverload
                if (!command.CanHandle() || !await command.CanHandleAsync())
                    continue;
                
                return command;
            }

            return null;
        }

        private static void AddCommandMetadataForAssembly(Assembly assembly, List<CommandMetadata> list)
        {
            var types = assembly.GetTypes();
            var commandTypes = types.Where(t =>
                t.IsClass && !t.IsAbstract && t.IsPublic && typeof(TelegramCommand).IsAssignableFrom(t));
            foreach (var commandType in commandTypes)
            {
                var commandMetadata = new CommandMetadata
                {
                    CommandType = commandType,
                    Attributes = commandType.GetCustomAttributesData()
                        .Where(it =>
                            !it.AttributeType.IsAbstract &&
                            typeof(TelegramBotFilterAttribute).IsAssignableFrom(it.AttributeType))
                        .Select(it => GetMetadataForAttribute(it.AttributeType)).ToList()
                };
                list.Add(commandMetadata);
            }
        }

        private static AttributeMetadata GetMetadataForAttribute(Type attributeType)
        {
            var attributeMetadata = new AttributeMetadata
            {
                AttributeType = attributeType,
                CreateAttributeFunc = serviceProvider =>
                    (TelegramBotFilterAttribute) ActivatorUtilities.CreateInstance(serviceProvider, attributeType)
            };
            return attributeMetadata;
        }
        
        private sealed class CommandMetadata
        {
            public Type CommandType { get; set; }
            public List<AttributeMetadata> Attributes { get; set; }
        }
        
        private sealed class AttributeMetadata
        {
            public Type AttributeType { get; set; }
            public Func<IServiceProvider, TelegramBotFilterAttribute> CreateAttributeFunc { get; set; }
        }
    }
}