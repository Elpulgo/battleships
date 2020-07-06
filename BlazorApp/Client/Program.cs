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
            builder.Services.AddSingleton<IEventService, EventService>();

            var host = builder.Build();

            var messageService = host.Services.GetRequiredService<IMessageService>();
            await messageService.InitializeAsync();

            await host.RunAsync();
        }
    }
}
