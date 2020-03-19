using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NBPCurrencyAggregator.API.Contracts
{
    public interface IServiceRegistration
    {
        void RegisterAppServices(IServiceCollection services, IConfiguration configuration);
    }
}
