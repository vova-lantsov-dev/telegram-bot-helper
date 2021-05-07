using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Telegram.Bot.Helper.DependencyInjection;

namespace Telegram.Bot.Helper.Attributes
{
    internal sealed class AttributeValidationContext
    {
        private readonly ReadOnlyCollection<AttributeFilterCommandManager.AttributeMetadata> _attributeMetadataInfo;
        private readonly List<Type> _handledTypes = new List<Type>();
        private readonly List<Type> _typesToHandle = new List<Type>();

        public AttributeValidationContext(IEnumerable<TelegramBotFilterAttribute> filters,
            ReadOnlyCollection<AttributeFilterCommandManager.AttributeMetadata> attributeMetadataInfo)
        {
            _attributeMetadataInfo = attributeMetadataInfo;
            var typesToHandle = new List<Type>();
        }

        private static IEnumerable<Type> GetAllFiltersRecursively(Type baseFilter)
        {
            yield return baseFilter;
        }
    }
}