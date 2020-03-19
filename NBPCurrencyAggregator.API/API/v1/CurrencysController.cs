using System;
using NBPCurrencyAggregator.API.Contracts;
using NBPCurrencyAggregator.API.Data;
using NBPCurrencyAggregator.API.Data.Entity;
using NBPCurrencyAggregator.API.DTO.Request;
using NBPCurrencyAggregator.API.DTO.Response;
using AutoMapper;
using AutoWrapper.Extensions;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace NBPCurrencyAggregator.API.API.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CurrencysController : ControllerBase
    {
        private readonly ILogger<CurrencysController> _logger;
        private readonly ICurrencyManager _currencyManager;
        private readonly IMapper _mapper;
        public CurrencysController(ICurrencyManager currencyManager, IMapper mapper, ILogger<CurrencysController> logger)
        {
            _currencyManager = currencyManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CurrencyQueryResponse>), Status200OK)]
        public async Task<IEnumerable<CurrencyQueryResponse>> Get()
        {
            var data = await _currencyManager.GetAllAsync();
            var currencys = _mapper.Map<IEnumerable<CurrencyQueryResponse>>(data);

            return currencys;
        }

        [Route("paged")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CurrencyQueryResponse>), Status200OK)]
        public async Task<IEnumerable<CurrencyQueryResponse>> Get([FromQuery] UrlQueryParameters urlQueryParameters)
        {
            var data = await _currencyManager.GetCurrenciesAsync(urlQueryParameters);
            var currencies = _mapper.Map<IEnumerable<CurrencyQueryResponse>>(data.Currencies);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(data.Pagination));

            return currencies;
        }

        [Route("{id:int}")]
        [HttpGet]
        [ProducesResponseType(typeof(CurrencyQueryResponse), Status200OK)]
        [ProducesResponseType(typeof(CurrencyQueryResponse), Status404NotFound)]
        public async Task<CurrencyQueryResponse> Get(int id)
        {
            var currency = await _currencyManager.GetByIdAsync(id);
            return currency != null ? _mapper.Map<CurrencyQueryResponse>(currency)
                                  : throw new ApiProblemDetailsException($"Record with id: {id} does not exist.", Status404NotFound);
        }

        [Route("fromto/{symbol}/{from}/{to}")]
        [HttpGet]
        public async Task<CurrencyQueryResponse> GetFromTo([FromQuery] string symbol, [FromQuery] DateTime from, [FromQuery] DateTime? to)
        {
            var currency = await _currencyManager.GetCurrenciesByDate(symbol, from, to);

            return currency != null ? _mapper.Map<CurrencyQueryResponse>(currency) 
                                  : throw new ApiProblemDetailsException($"No data found.", Status404NotFound);

        }

        [Route("fromtoavg/{symbol}/{from}/{to}")]
        [HttpGet]
        public async Task<string> GetFromToAvg([FromQuery] string symbol, [FromQuery] DateTime from, [FromQuery] DateTime? to)
        {
            var currency = await _currencyManager.GetCurrenciesByDate(symbol, from, to);

            var ask = currency.Rates.Select(r => r.Ask);
            var bid = currency.Rates.Select(r => r.Bid);

            return $"{symbol} Ask average: {ask.Average()}, Bid average: {bid.Average()}";
        }
    }
}