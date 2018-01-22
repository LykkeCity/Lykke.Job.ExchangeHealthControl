using System.Threading.Tasks;
using Lykke.Job.ExchangeHealthControl.Core.Domain;

namespace Lykke.Job.ExchangeHealthControl.Core.Repositories
{
    public interface IExchangeHealthControlResultRepository
    {
        Task InsertOrUpdateAsync(ExchangeHealthControlResult obj);
    }
}
