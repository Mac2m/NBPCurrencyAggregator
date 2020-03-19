using System;
using System.Collections.Generic;
using NBPCurrencyAggregator.API.Data.Entity;

namespace NBPCurrencyAggregator.API.DTO.Response
{
    public class CurrencyQueryResponse
    {
        public int Id { get; set; }
        public string Table { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<Rate> Rates { get; set; }

    }
}
