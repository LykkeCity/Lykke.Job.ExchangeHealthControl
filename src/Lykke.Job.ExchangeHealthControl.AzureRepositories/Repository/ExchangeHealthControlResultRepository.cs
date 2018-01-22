using AzureStorage;
using Lykke.Job.ExchangeHealthControl.AzureRepositories.Entity;
using Lykke.Job.ExchangeHealthControl.Core.Domain;
using Lykke.Job.ExchangeHealthControl.Core.Repositories;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories.Repository
{
    public class ExchangeHealthControlResultRepository 
        : GenericAzureRepository<ExchangeHealthControlResult, ExchangeHealthControlResultEntity>, 
            IExchangeHealthControlResultRepository
    {
        public ExchangeHealthControlResultRepository(INoSQLTableStorage<ExchangeHealthControlResultEntity> tableStorage)
            : base(tableStorage)
        {
        }
    }
}
