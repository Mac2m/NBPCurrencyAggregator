using FluentValidation;
using System;
using System.Collections.Generic;
using NBPCurrencyAggregator.API.Data.Entity;
using NBPCurrencyAggregator.API.Infrastructure.Helpers;

namespace NBPCurrencyAggregator.API.DTO.Request
{
    public class CreateCurrencyRequest
    {
        public string Table { get; set; }
        public string CurrencyName { get; set; }
        public string Code { get; set; }
        public DateTime EffectiveDate { get; set; }
        public List<Rate> Rates { get; set; }

    }

    public class CreateCurrencyRequestValidator : AbstractValidator<CreateCurrencyRequest>
    {
        public CreateCurrencyRequestValidator()
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
