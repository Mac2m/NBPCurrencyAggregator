using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NBPCurrencyAggregator.API.Data.Entity;

namespace NBPCurrencyAggregator.API.Contracts
{
    public interface INbpApiClient
    {
        Task GetCurrencyTodaysRatesAsync(string symbol, CancellationToken stoppingToken);

        Task<Currency> GetCurrencyFromTo(string symbol, DateTime from, DateTime? to);
    }
}
