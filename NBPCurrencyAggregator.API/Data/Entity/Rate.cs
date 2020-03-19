using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NBPCurrencyAggregator.API.Data.Entity
{
    public class Rate : EntityBase
    {
        public int Id { get; set; }
        public string No { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
    }
}
