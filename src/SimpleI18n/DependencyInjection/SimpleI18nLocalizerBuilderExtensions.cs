using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace SimpleI18n.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring I18n Localization services.
    /// </summary>
    public static class SimpleI18nLocalizerBuilderExtensions
    {
        public static void AddSimpleI18n(this IServiceCollection services)
        {
            services.Add(ServiceDescriptor.
                Singleton<IStringLocalizer, SimpleI18nStringLocalizer>());
        }
    }
}