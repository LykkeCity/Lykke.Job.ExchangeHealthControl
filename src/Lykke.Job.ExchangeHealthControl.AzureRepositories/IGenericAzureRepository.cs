using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories
{
    public interface IGenericAzureRepository<TD, TE>
        where TD : class
        where TE : MappableTableEntity<TD, TE>, new()
    {
        Task InsertAsync(TD obj);
        Task InsertOrUpdateAsync(TD obj);
        Task InsertOrMergeAsync(TD obj);
        Task InsertOrMergeBatchAsync(IEnumerable<TD> objects);
        Task InsertOrReplaceBatchAsync(IEnumerable<TD> objects);
        Task<IEnumerable<TD>> RetrieveAllAsync();
        Task<bool> DeleteAsync(string partitionKey, string rowKey);
        Task ClearTableAsync();

        bool Any();
    }
}
