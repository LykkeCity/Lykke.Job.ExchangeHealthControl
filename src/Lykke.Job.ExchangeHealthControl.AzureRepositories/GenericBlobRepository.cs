using System.Text;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Blob;
using Common;
using Lykke.Job.ExchangeHealthControl.Core;
using Lykke.Job.ExchangeHealthControl.Core.Repositories;
using Lykke.SettingsReader;
using Newtonsoft.Json;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories
{
    public class GenericBlobRepository : IGenericBlobRepository
    {
        private readonly IBlobStorage _blobStorage;

        /// <summary>
        /// For mocking purposes
        /// </summary>
        public GenericBlobRepository()
        {
        }

        public GenericBlobRepository(IReloadingManager<string> connectionString)
        {
            _blobStorage = AzureBlobStorage.Create(connectionString);
        }

        public T Read<T>(string blobContainer, string key)
        {
            return ReadAsync<T>(blobContainer, key).Result;
        }

        public async Task<T> ReadAsync<T>(string blobContainer, string key)
        {
            if (!_blobStorage.HasBlobAsync(blobContainer, key).Result) 
                return default(T);
            
            var data = (await _blobStorage.GetAsync(blobContainer, key)).ToBytes();
            var str = Encoding.UTF8.GetString(data);

            return JsonConvert.DeserializeObject<T>(str);
        }

        public async Task Write<T>(string blobContainer, string key, T obj)
        {
            var data = JsonConvert.SerializeObject(obj).ToUtf8Bytes();
            await _blobStorage.SaveBlobAsync(blobContainer, key, data);
        }
    }
}
