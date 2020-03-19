using NBPCurrencyAggregator.API.Contracts;
using NBPCurrencyAggregator.API.Data.DataManager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NBPCurrencyAggregator.API.Infrastructure.Installers
{
    internal class RegisterContractMappings : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration config)
        {
            //Register Interface Mappings for Repositories
            services.AddTransient<ICurrencyManager, CurrencyManager>();
        }
    }
}
