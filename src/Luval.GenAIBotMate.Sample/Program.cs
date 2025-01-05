using Luval.GenAIBotMate.Infrastructure.Configuration;
using Luval.GenAIBotMate.Infrastructure.Data;
using Luval.GenAIBotMate.Infrastructure.Interfaces;
using Luval.GenAIBotMate.Sample.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace Luval.GenAIBotMate.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var config = builder.Configuration;

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddFluentUIComponents();

            //TODO: Add secrests for the OpenAI key, Azure Blob Storage connection string
            builder.Services.AddGenAIBotServicesDefault(
                config.GetValue<string>("OpenAIKey"),
                config.GetValue<string>("AzureConnectionString")
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Initialize the database
            GenAIBotContextHelper.InitializeAsync(new SqliteChatDbContext())
                .GetAwaiter()
                .GetResult();

            app.Run();
        }
    }
}
