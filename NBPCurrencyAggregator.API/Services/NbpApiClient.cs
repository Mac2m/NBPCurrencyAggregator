using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NBPCurrencyAggregator.API.Contracts;
using NBPCurrencyAggregator.API.Data.Entity;
using Newtonsoft.Json;

namespace NBPCurrencyAggregator.API.Services
{
    public class NbpApiClient : INbpApiClient
    {
        private readonly ILogger<NbpApiClient> _logger;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializer _serializer;
        private readonly ICurrencyManager _currencyManager;


        public NbpApiClient(ILogger<NbpApiClient> logger, HttpClient httpClient, JsonSerializer serializer, ICurrencyManager currencyManager)
        {
            _logger = logger;
            _httpClient = httpClient;
            _serializer = serializer;
            _currencyManager = currencyManager;
        }

        public async Task GetCurrencyTodaysRatesAsync(string symbol, CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var response =
                        await _httpClient.GetAsync($"a/{symbol}/today/?format=json", stoppingToken);
                    if (response.IsSuccessStatusCode == false)
                    {
                        _logger.LogCritical("NBP API Failed with HTTP Status Code {statusCode} at: {time}",
                            response.StatusCode, DateTimeOffset.Now);
                        continue;
                    }

                    using var sr = new StreamReader(await response.Content.ReadAsStreamAsync());
                    using var jsonTextReader = new JsonTextReader(sr);
                    var exchangeRateResult = _serializer.Deserialize<Currency>(jsonTextReader);

                    await _currencyManager.CreateAsync(exchangeRateResult);

                    if (exchangeRateResult.Rates.Any())
                    {
                        foreach (var rate in exchangeRateResult.Rates)
                        {
                            _logger.LogInformation($"{symbol} = Bid: {rate.Bid}, Ask: {rate.Ask}");
                        }
                    }
                    else
                    {
                        _logger.LogCritical($"Exchange rate not returned from API.");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogCritical($"{nameof(HttpRequestException)}: {ex.Message}");
            }
        }

        public async Task<Currency> GetCurrencyFromTo(string symbol, DateTime from, DateTime? to)
        {
            var response = await _httpClient.GetAsync($"{symbol}/{from}/{to}/?format=json");
            if (response.IsSuccessStatusCode == false)
            {
                _logger.LogCritical("NBP API Failed with HTTP Status Code {statusCode} at: {time}",
                    response.StatusCode, DateTimeOffset.Now);
            }

            using var sr = new StreamReader(await response.Content.ReadAsStreamAsync());
            using var jsonTextReader = new JsonTextReader(sr);
            var result = _serializer.Deserialize<Currency>(jsonTextReader);

            return result;
        }
    }
}
