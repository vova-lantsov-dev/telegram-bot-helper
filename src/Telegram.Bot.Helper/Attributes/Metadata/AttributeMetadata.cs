using System;
using System.Collections.Generic;

namespace Telegram.Bot.Helper.Attributes.Metadata
{
    internal sealed class AttributeMetadata
    {
        public Type AttributeType { get; set; }
        public List<AttributeMetadata> DependsOnFilters { get; set; }
        public Func<IServiceProvider, TelegramBotFilterAttribute> CreateAttributeFunc { get; set; }
    }
}