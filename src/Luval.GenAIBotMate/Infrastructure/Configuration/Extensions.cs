﻿using Luval.AuthMate.Core.Interfaces;
using Luval.AuthMate.Core.Resolver;
using Luval.GenAIBotMate.Core.Services;
using Luval.GenAIBotMate.Infrastructure.Data;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GenAIBotMate.Infrastructure.Configuration
{
    /// <summary>
    /// Extension methods for the services
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Adds general AI Bot services to the service collection.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotServices(this IServiceCollection s)
        {
            s.AddHttpContextAccessor();
            s.AddScoped<IUserResolver, WebUserResolver>();
            return s;
        }

        /// <summary>
        /// Adds OpenAI services to the service collection.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="openAIKey">The OpenAI API key.</param>
        /// <param name="openAIModel">The OpenAI model to use. Default is "gpt-4o".</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotOpenAIServices(this IServiceCollection s, string openAIKey, string openAIModel = "gpt-4o")
        {
            s.AddScoped<IChatCompletionService>((i) =>
            {
                var kernel = Kernel.CreateBuilder()
                            .AddOpenAIChatCompletion(openAIModel, openAIKey)
                            .Build();
                return kernel.GetRequiredService<IChatCompletionService>();
            });
            return s;
        }

        /// <summary>
        /// Adds Azure Media services to the service collection.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="storageConnectionString">The Azure storage connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotAzureMediaServices(this IServiceCollection s, string storageConnectionString)
        {
            s.AddScoped<MediaServiceConfig>((i) =>
            {
                return new MediaServiceConfig()
                {
                    ConnectionString = storageConnectionString,
                    ProviderName = "Azure",
                    ContainerName = "genaibot",
                    SASExpiration = TimeSpan.FromHours(1)
                };
            });
            s.AddScoped<IMediaService, AzureMediaService>();
            return s;
        }

        /// <summary>
        /// Adds storage services to the service collection.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="postgresConnectionString">The PostgreSQL connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotStorageServices(this IServiceCollection s, string postgresConnectionString)
        {
            s.AddScoped<IChatDbContext, PostgresChatDbContext>((i) =>
            {
                return new PostgresChatDbContext(postgresConnectionString);
            });
            return s;
        }

        /// <summary>
        /// Adds default services to the service collection.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="openAIKey">The OpenAI API key.</param>
        /// <param name="postgresConnectionString">The PostgreSQL connection string.</param>
        /// <param name="azureStorageConnectionString">The Azure storage connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotDefaultServices(this IServiceCollection s, string openAIKey, string postgresConnectionString, string azureStorageConnectionString)
        {
            s.AddGenAIBotServices();
            s.AddGenAIBotOpenAIServices(openAIKey);
            s.AddGenAIBotAzureMediaServices(azureStorageConnectionString);
            s.AddGenAIBotStorageServices(postgresConnectionString);
            return s;
        }
    }
}