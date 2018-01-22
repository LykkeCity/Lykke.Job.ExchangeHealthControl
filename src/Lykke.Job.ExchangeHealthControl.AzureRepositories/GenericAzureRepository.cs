using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AzureStorage;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories
{
    public abstract class GenericAzureRepository<TD, TE> : IGenericAzureRepository<TD, TE>
        where TD : class
        where TE : MappableTableEntity<TD, TE>, new()
    {
        protected readonly INoSQLTableStorage<TE> _tableStorage;

        public GenericAzureRepository(INoSQLTableStorage<TE> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        public virtual async Task InsertAsync(TD obj)
        {
            await _tableStorage.InsertAsync(InvokeCreate(obj));
        }

        public virtual async Task InsertOrUpdateAsync(TD obj)
        {
            await _tableStorage.InsertOrReplaceAsync(InvokeCreate(obj));
        }

        public virtual async Task InsertOrMergeAsync(TD obj)
        {
            await _tableStorage.InsertOrMergeAsync(InvokeCreate(obj));
        }

        public virtual async Task InsertOrMergeBatchAsync(IEnumerable<TD> objects)
        {
            await _tableStorage.InsertOrMergeBatchAsync(objects.Select(InvokeCreate));
        }

        public virtual async Task InsertOrReplaceBatchAsync(IEnumerable<TD> objects)
        {
            await _tableStorage.InsertOrReplaceBatchAsync(objects.Select(InvokeCreate));
        }

        public virtual async Task<IEnumerable<TD>> RetrieveAllAsync()
        {
            return (await _tableStorage.GetDataAsync()).Select(InvokeRestore);
        }

        public virtual async Task<bool> DeleteAsync(string partitionKey, string rowKey)
        {
            return await _tableStorage.DeleteIfExistAsync(partitionKey, rowKey);
        }

        /// <summary>
        /// Delete everything in the table. Use with caution!!!
        /// </summary>
        /// <returns></returns>
        public virtual async Task ClearTableAsync()
        {
            var data = await _tableStorage.GetDataAsync();
            await Task.WhenAll(data.Select(obj => _tableStorage.DeleteAsync(obj)));
        }

        public bool Any()
        {
            return _tableStorage.Any();
        }


        #region reflection methods

        private static TE InvokeCreate(TD obj)
        {
            var methodName = nameof(MappableTableEntity<TD, TE>.Create);
            var overridedMethod = typeof(TE).GetMethod(methodName,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static);
            var callableMethod = overridedMethod ?? typeof(MappableTableEntity<TD, TE>).GetMethod(methodName);
            return (TE) callableMethod.Invoke(null, new object[] {obj});
        }

        private static TD InvokeRestore(TE entity)
        {
            var methodName = nameof(MappableTableEntity<TD, TE>.Restore);
            var overridedMethod = typeof(TE).GetMethod(methodName,
                BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static);
            var callableMethod = overridedMethod ?? typeof(MappableTableEntity<TD, TE>).GetMethod(methodName);
            return (TD) callableMethod.Invoke(null, new object[] {entity});
        }

        #endregion reflection methods
    }
}
