﻿using System;
using System.Threading.Tasks;
using DestinyBot.Data;
using DestinyBot.Jobs;
using DestinyBot.Models;
using DestinyBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace DestinyBot
{
    public class DestinyBot
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;

        public DestinyBot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 5000,
                AlwaysDownloadUsers = true,
#if DEBUG
                LogLevel = LogSeverity.Verbose,
#else
                LogLevel = LogSeverity.Info,
#endif
                DefaultRetryMode = RetryMode.AlwaysRetry
            });

            _config = BuildConfig();
            _services = ConfigureServices();
        }

        public async Task StartAsync()
        {
            _client.Log += BotLogHook.Log;

            await _client.LoginAsync(TokenType.Bot, _config.Get<BotConfig>().DiscordToken);

            await _client.StartAsync();

            await _services.GetRequiredService<CommandHandlingService>().InitializeAsync(_services);
            SetupJobs();
            await Task.Delay(-1);
        }


        private void SetupJobs()
        {
            var registry = new Registry();
            using (var db = _services.GetRequiredService<DestinyBotContext>())
            {
                db.Database.Migrate();
                
                foreach (var subscription in db.YoutubeSubscriptions)
                {
                    registry.Schedule(() => new YoutubeJob(
                            subscription.ChannelName,
                            _services.GetService<YoutubeService>(),
                            _client,
                            _services.GetService<DestinyBotContext>()))
                        .WithName(subscription.ChannelName + subscription.DiscordChannelId)
                        .ToRunNow().AndEvery(5)
                        .Seconds();
                }
            }
            JobManager.Initialize(registry);
            JobManager.JobException += info => Log.Information(info.Exception, "{jobName} has a problem", info.Name);

        }

        private IConfiguration BuildConfig() => new ConfigurationBuilder()
            .AddEnvironmentVariables(string.Empty)
            .Build();

        private IServiceProvider ConfigureServices()
        {
            var config = _config.Get<BotConfig>();

            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .Configure<BotConfig>(_config)
                .AddSingleton(new YoutubeService(config.YoutubeKey))
                .AddDbContext<DestinyBotContext>(ServiceLifetime.Transient)
                //We delegate the config object so we dont have to use IOptionsSnapshot<T> or IOptions<T> to get the Config
                .AddTransient(provider => provider.GetRequiredService<IOptions<BotConfig>>().Value)
                .BuildServiceProvider();
        }
    }
}