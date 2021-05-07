using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot.Helper.Options;

namespace Telegram.Bot.Helper.Attributes.Metadata
{
    internal sealed class AttributeMetadataLoader
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TelegramBotOptions _options;
        private readonly List<AttributeMetadata> _metadataList = new List<AttributeMetadata>();

        public AttributeMetadataLoader(IServiceProvider serviceProvider,
            IOptions<TelegramBotOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public void LoadAttributes()
        {
            var assembliesToLoadFrom = _options.Assemblies?.ToList() ?? new List<Assembly>();

            var currentAssembly = Assembly.GetExecutingAssembly();
            if (!assembliesToLoadFrom.Contains(currentAssembly))
            {
                assembliesToLoadFrom.Add(currentAssembly);
            }

            foreach (var assembly in assembliesToLoadFrom)
            {
                LoadAttributesFromAssembly(assembly);
            }
        }

        private void LoadAttributesFromAssembly(Assembly assembly)
        {
            var assemblyTypes = assembly.GetTypes();
            var attributeTypes = assemblyTypes.Where(t =>
                t.IsClass && t.IsPublic && !t.IsAbstract && typeof(TelegramBotFilterAttribute).IsAssignableFrom(t));

            foreach (var attributeType in attributeTypes)
            {
                // Skip all attributes that were already loaded into the '_metadataList'
                if (_metadataList.Any(it => it.AttributeType == attributeType))
                    continue;
                
                var attributeMetadata = LoadAttributeFromAssemblyRecursively(attributeType);
                _metadataList.Add(attributeMetadata);
            }
        }

        private AttributeMetadata LoadAttributeFromAssemblyRecursively(Type attributeType)
        {
            var dependsOnFilters = new List<AttributeMetadata>();
            var customAttributes = attributeType.GetCustomAttributes<TelegramBotFilterAttribute>(true);
            foreach (var customAttribute in customAttributes)
            {
                var customAttributeType = customAttribute.GetType();
                if (customAttributeType.IsDefined(attributeType, true))
                {
                    throw new Exception("Circular filter dependency was detected.");
                }

                var dependencyAttributeMetadata = LoadAttributeFromAssemblyRecursively(customAttributeType);
                dependsOnFilters.Add(dependencyAttributeMetadata);
            }

            var attributeMetadata = new AttributeMetadata
            {
                AttributeType = attributeType,
                CreateAttributeFunc = provider =>
                    (TelegramBotFilterAttribute) ActivatorUtilities.CreateInstance(provider, attributeType),
                DependsOnFilters = dependsOnFilters
            };
            return attributeMetadata;
        }
    }
}