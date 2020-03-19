using AutoMapper;
using NBPCurrencyAggregator.API.Data.Entity;
using NBPCurrencyAggregator.API.DTO;
using NBPCurrencyAggregator.API.DTO.Response;
using NBPCurrencyAggregator.API.DTO.Request;

namespace NBPCurrencyAggregator.API.Infrastructure.Configs
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<Currency, CreateCurrencyRequest>().ReverseMap();
            CreateMap<Currency, UpdateCurrencyRequest>().ReverseMap();
            CreateMap<Currency, CurrencyQueryResponse>().ReverseMap();
        }
    }
}
