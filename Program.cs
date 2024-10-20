using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using ModMail;

namespace ModMailMain
{
    class Program
    {
        static async Task Main()
        {
            #pragma warning disable CA1416 // Validate platform compatibility
            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Settings.json", false, true)
                    .Build();

                    x.AddConfiguration(configuration);
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug);
                })
                .ConfigureDiscordHost((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Critical,
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 1000,
                        GatewayIntents = GatewayIntents.All,
                    };
                    config.Token = context.Configuration["Token"];
                })
                .UseCommandService((context, config) =>
                {
                    config.CaseSensitiveCommands = false;
                    config.LogLevel = LogSeverity.Error;
                    config.DefaultRunMode = RunMode.Sync;
                })
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<CommandHandler>();
                })
                .UseConsoleLifetime();
            #pragma warning restore CA1416 // Validate platform compatibility

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }

        }
    }
}