using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NBPCurrencyAggregator.API.Data.Entity
{
    public class Currency : EntityBase
    {
        public int Id { get; set; }
        public string Table { get; set; }
        [JsonProperty("currency")]
        public string CurrencyName { get; set; }
        public string Code { get; set; }
        public List<Rate> Rates { get; set; }
    }
}
