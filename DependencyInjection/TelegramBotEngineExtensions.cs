using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Helper.Options;

namespace Telegram.Bot.Helper.DependencyInjection
{
    public static class TelegramBotEngineExtensions
    {
        public static IServiceCollection AddTelegramBotEngine(this IServiceCollection services,
            Action<TelegramBotOptions> configure)
        {
            services.Configure(configure);
            services.AddSingleton<ITelegramBotCommandManager, AttributeFilterCommandManager>();
            services.AddSingleton<IUpdateHandler, DependencyInjectionUpdateHandler>();
            return services;
        }
        
        public static IServiceCollection AddTelegramBotEngine(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<TelegramBotOptions>(configuration.GetSection("TelegramBot"));
            services.AddSingleton<ITelegramBotCommandManager, AttributeFilterCommandManager>();
            services.AddSingleton<IUpdateHandler, DependencyInjectionUpdateHandler>();
            return services;
        }
    }
}