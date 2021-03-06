using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BlazorApp.Client.Services;

namespace BlazorApp.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddSingleton<IMessageService, MessageService>();
            builder.Services.AddSingleton<IGamePlayService, GamePlayService>();

            builder.Services.AddSingleton<EventService>();
            builder.Services.AddSingleton<IEventExecutor>(provider => provider.GetRequiredService<EventService>());
            builder.Services.AddSingleton<IEventListener>(provider => provider.GetRequiredService<EventService>());

            var host = builder.Build();
            await host.PreLoad();
            await host.RunAsync();
        }
    }

    public static class WebAssemblyHostExtensions
    {
        public static async Task PreLoad(this WebAssemblyHost host)
        {
            var messageService = host.Services.GetRequiredService<IMessageService>();
            await messageService.InitializeAsync();

            var gamePlayService = host.Services.GetRequiredService<IGamePlayService>();
            await gamePlayService.PreLoadPlayerSlotAvailable();
        }
    }
}
