﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Translumo.Configuration;
using Translumo.Infrastructure.Encryption;
using Translumo.OCR.Configuration;
using Translumo.Translation.Configuration;

namespace Translumo.Utils
{
    public static class ServiceCollectionExtensions
    {
        public static void AddConfigurationStorage(this ServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ConfigurationStorage>(sc =>
            {
                var confStorage = new ConfigurationStorage(sc, sc.GetService<IEncryptionService>(), sc.GetService<ILogger<ConfigurationStorage>>());
                confStorage.RegisterConfiguration<ChatWindowConfiguration>();
                confStorage.RegisterConfiguration<OcrGeneralConfiguration>();
                confStorage.RegisterConfiguration<TranslationConfiguration>();
                confStorage.RegisterConfiguration<SystemConfiguration>();

                return confStorage;
            });
        }
    }
}
