using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBPCurrencyAggregator.API.Contracts;
using NBPCurrencyAggregator.API.Data.Entity;
using Newtonsoft.Json;

namespace NBPCurrencyAggregator.API.Infrastructure.Workers
{
    public class NbpApiCallerWorker : BackgroundService
    {
        private const string UsdSymbol = "usd";
        private const string EurSymbol = "eur";

        private readonly ILogger<NbpApiCallerWorker> _logger;
        private readonly INbpApiClient _client;

        public NbpApiCallerWorker(ILogger<NbpApiCallerWorker> logger, INbpApiClient client)
        {
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var currentTime = DateTime.UtcNow;
                if (currentTime.Hour == 12 && currentTime.Minute == 20 && currentTime.Second == 0)
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                    await _client.GetCurrencyTodaysRatesAsync(UsdSymbol, stoppingToken);
                    await _client.GetCurrencyTodaysRatesAsync(EurSymbol, stoppingToken);
                }
                
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

    }
}
