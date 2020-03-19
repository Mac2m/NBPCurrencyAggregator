using NBPCurrencyAggregator.API.Infrastructure.Helpers;
using FluentValidation;
using System;
using System.Collections.Generic;
using NBPCurrencyAggregator.API.Data.Entity;

namespace NBPCurrencyAggregator.API.DTO.Request
{
    public class UpdateCurrencyRequest
    {
        public string Table { get; set; }
        public string CurrencyName { get; set; }
        public string Code { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<Rate> Rates { get; set; }

    }

    public class UpdateCurrencyRequestValidator : AbstractValidator<UpdateCurrencyRequest>
    {
        public UpdateCurrencyRequestValidator()
        {
            RuleFor(o => o.Table).NotEmpty();
            RuleFor(o => o.CurrencyName).NotEmpty();
            RuleFor(o => o.Code).NotEmpty();
            RuleFor(o => o.EffectiveDate)
                .NotEmpty()
                    .Must(PropertyValidation.IsValidDateTime);
            RuleFor(o => o.Rates).NotEmpty();
        }
    }
}
