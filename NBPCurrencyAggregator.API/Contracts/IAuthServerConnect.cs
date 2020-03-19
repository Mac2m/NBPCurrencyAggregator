using System.Threading.Tasks;

namespace NBPCurrencyAggregator.API.Contracts
{
    public interface IAuthServerConnect
    {
        Task<string> RequestClientCredentialsTokenAsync();
    }
}
