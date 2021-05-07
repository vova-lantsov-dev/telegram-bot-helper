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
        private readonly ReadOnlyCollection<AttributeMetadata> _attributeMetadataInfo;
        
        internal IServiceProvider ServiceProvider { private get; set; }

        public AttributeFilterCommandManager(
            IOptions<TelegramBotOptions> options)
        {
            Assembly[] assemblyTypes = options.Value.Assemblies;
            
            var metadataInfo = new List<CommandMetadata>();
            var attributeMetadataInfo = new List<AttributeMetadata>();
            foreach (var assembly in assemblyTypes)
            {
                AddCommandMetadataForAssembly(assembly, metadataInfo);
                AddAttributeMetadataForAssembly(assembly, attributeMetadataInfo);
            }
            _metadataInfo = metadataInfo.AsReadOnly();
            _attributeMetadataInfo = attributeMetadataInfo.AsReadOnly();
        }
        
        public async ValueTask<TelegramCommand> GetCommandForUpdateAsync(Update update, CancellationToken ct)
        {
            IServiceScope scope = ServiceProvider.CreateScope();
            foreach (var commandMetadata in _metadataInfo)
            {
                bool skip = false;
                foreach (var filterAttributeType in commandMetadata.FilterAttributes)
                {
                    var attributeMetadata =
                        _attributeMetadataInfo.FirstOrDefault(it => it.AttributeType == filterAttributeType);
                    if (attributeMetadata == null)
                        // TODO
                        throw new NotImplementedException();
                    var filterAttribute = attributeMetadata.CreateAttributeFunc(ServiceProvider);
                    // ReSharper disable once MethodHasAsyncOverload
                    if (!filterAttribute.IsValid() || !await filterAttribute.IsValidAsync())
                        skip = true;
                }
                if (skip)
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
                    FilterAttributes = commandType.GetCustomAttributesData()
                        .Select(it => it.AttributeType)
                        .Where(t => typeof(TelegramBotFilterAttribute).IsAssignableFrom(t))
                        .ToList()
                };
                list.Add(commandMetadata);
            }
        }

        private static void AddAttributeMetadataForAssembly(Assembly assembly, List<AttributeMetadata> list)
        {
            var types = assembly.GetTypes();
            var attributeFilterTypes = types.Where(t =>
                t.IsClass && !t.IsAbstract && t.IsPublic && typeof(TelegramBotFilterAttribute).IsAssignableFrom(t));
            foreach (var attributeFilterType in attributeFilterTypes)
            {
                if (list.Any(it => it.AttributeType == attributeFilterType))
                    continue;
                var attributeFilterMetadata = GetMetadataForAttribute(attributeFilterType);
                list.Add(attributeFilterMetadata);
            }
        }

        private static AttributeMetadata GetMetadataForAttribute(Type attributeType)
        {
            var attributeMetadata = new AttributeMetadata
            {
                AttributeType = attributeType,
                CreateAttributeFunc = serviceProvider =>
                    (TelegramBotFilterAttribute) ActivatorUtilities.CreateInstance(serviceProvider, attributeType),
                ParentAttributes = attributeType.GetCustomAttributesData().Select(it => it.AttributeType)
                    .Where(t => typeof(TelegramBotFilterAttribute).IsAssignableFrom(t)).ToList()
            };
            return attributeMetadata;
        }
        
        private sealed class CommandMetadata
        {
            public Type CommandType { get; set; }
            public List<Type> FilterAttributes { get; set; }
        }

        internal sealed class AttributeMetadata
        {
            public Type AttributeType { get; set; }
            public List<Type> ParentAttributes { get; set; }
            public Func<IServiceProvider, TelegramBotFilterAttribute> CreateAttributeFunc { get; set; }
        }
    }
}