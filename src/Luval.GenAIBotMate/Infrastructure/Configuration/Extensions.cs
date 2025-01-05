using Luval.AuthMate.Core.Interfaces;
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
using System.Diagnostics;
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
            s.AddScoped<IGenAIBotStorageService, GenAIBotStorageService>();
            s.AddScoped<GenAIBotService>();
            return s;
        }

        /// <summary>
        /// Adds the Semantic Kernel to the service collection using a factory method.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="factory">The factory method to create the Kernel instance.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotSemanticKernel(this IServiceCollection s, Func<IServiceProvider, Kernel> factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            s.AddScoped<Kernel>((i) =>
            {
                return factory?.Invoke(i);
            });
            s.AddScoped<IChatCompletionService>((i) => {
                var kernel = i.GetRequiredService<Kernel>();
                return kernel.GetRequiredService<IChatCompletionService>();
            });
            return s;
        }

        /// <summary>
        /// Adds the Semantic Kernel to the service collection using OpenAI credentials.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="openAIKey">The OpenAI API key.</param>
        /// <param name="openAIModel">The OpenAI model to use. Default is "gpt-4o".</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotSemanticKernel(this IServiceCollection s, string openAIKey, string openAIModel = "gpt-4o")
        {
            return s.AddGenAIBotSemanticKernel((i) =>
            {
                return Kernel.CreateBuilder()
                            .AddOpenAIChatCompletion(openAIModel, openAIKey)
                            .Build();
            });
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
        public static IServiceCollection AddGenAIBotPostgresStorageServices(this IServiceCollection s, string postgresConnectionString)
        {
            s.AddScoped<IChatDbContext, PostgresChatDbContext>((i) =>
            {
                return new PostgresChatDbContext(postgresConnectionString);
            });
            return s;
        }

        /// <summary>
        /// Adds storage services to the service collection.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="sqliteConnectionString">The Sqlite connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotSqliteStorageServices(this IServiceCollection s, string sqliteConnectionString = "Data Source=botmate.db")
        {
            s.AddScoped<IChatDbContext, SqliteChatDbContext>((i) =>
            {
                return new SqliteChatDbContext(sqliteConnectionString);
            });
            return s;
        }

        /// <summary>
        /// Adds default services to the service collection with a Postgres database.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="openAIKey">The OpenAI API key.</param>
        /// <param name="sqliteConnectionString">The PostgreSQL connection string.</param>
        /// <param name="azureStorageConnectionString">The Azure storage connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotServicesWithSqlite(this IServiceCollection s, string openAIKey, string sqliteConnectionString, string azureStorageConnectionString)
        {
            s.AddGenAIBotServices();
            s.AddGenAIBotSemanticKernel(openAIKey);
            s.AddGenAIBotAzureMediaServices(azureStorageConnectionString);
            s.AddGenAIBotSqliteStorageServices(sqliteConnectionString);
            return s;
        }

        /// <summary>
        /// Adds default services to the service collection with a Sqlite database.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="openAIKey">The OpenAI API key.</param>
        /// <param name="sqliteConnectionString">The Sqlite connection string.</param>
        /// <param name="azureStorageConnectionString">The Azure storage connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotServicesWithPostgres(this IServiceCollection s, string openAIKey, string sqliteConnectionString, string azureStorageConnectionString)
        {
            s.AddGenAIBotServices();
            s.AddGenAIBotSemanticKernel(openAIKey);
            s.AddGenAIBotAzureMediaServices(azureStorageConnectionString);
            s.AddGenAIBotPostgresStorageServices(sqliteConnectionString);
            return s;
        }

        /// <summary>
        /// Adds default services to the service collection with a Sqlite database.
        /// </summary>
        /// <param name="s">The service collection.</param>
        /// <param name="openAIKey">The OpenAI API key.</param>
        /// <param name="azureStorageConnectionString">The Azure storage connection string.</param>
        /// <param name="sqliteConnectionString">The Sqlite connection string.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddGenAIBotServicesDefault(this IServiceCollection s, string openAIKey, string azureStorageConnectionString, string sqliteConnectionString = "Data Source=botmate.db")
        {
            s.AddGenAIBotServices();
            s.AddGenAIBotSemanticKernel(openAIKey);
            s.AddGenAIBotAzureMediaServices(azureStorageConnectionString);
            s.AddGenAIBotSqliteStorageServices(sqliteConnectionString);
            return s;
        }
    }
}
