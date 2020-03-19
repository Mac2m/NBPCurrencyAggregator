using System;
using NBPCurrencyAggregator.API.Data;
using NBPCurrencyAggregator.API.Data.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NBPCurrencyAggregator.API.Contracts
{
    public interface ICurrencyManager : IRepository<Currency>
    {
        Task<(IEnumerable<Currency> Currencies, Pagination Pagination)> GetCurrenciesAsync(UrlQueryParameters urlQueryParameters);

        Task<Currency> GetCurrenciesByDate(string symbol, DateTime from, DateTime? to);
        //Add more class specific methods here when neccessary
    }
}
