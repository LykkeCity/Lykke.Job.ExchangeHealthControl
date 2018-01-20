using System;
using AzureStorage.Tables;
using Common.Log;
using Lykke.Job.ExchangeHealthControl.AzureRepositories.Entity;
using Lykke.Job.ExchangeHealthControl.AzureRepositories.Repository;
using Lykke.Job.ExchangeHealthControl.Core.Domain;
using Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings;
using Lykke.SettingsReader;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories
{
    public class AzureRepoFactory
    {
        private readonly IReloadingManager<ExchangeHealthControlJobSettings> _reloadingManager;
        private readonly ILog _log;

        public AzureRepoFactory(IReloadingManager<ExchangeHealthControlJobSettings> reloadingManager, ILog log)
        {
            _reloadingManager = reloadingManager;
            _log = log;
        }

        /// <summary>
        /// Creates new repository of type TR.
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <typeparam name="TD"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private TR GetRepository<TR, TD, TE>(string tableName)
            where TR : GenericAzureRepository<TD, TE>
            where TD : class, new()
            where TE : MappableTableEntity<TD, TE>, new()
        {
            var storage = AzureTableStorage<TE>.Create(_reloadingManager.Nested(x => x.Db.StorageConnString),
                tableName,
                _log,
                new TimeSpan(1, 0, 0)
            );

            return (TR) Activator.CreateInstance(typeof(TR), storage);
        }

        public GenericBlobRepository GetGenericBlobRepository()
        {
            return new GenericBlobRepository(_reloadingManager.Nested(x => x.Db.StorageConnString));
        }

        public ExchangeHealthControlResultRepository GetExchangeHealthControlResultRepository()
        {
            return GetRepository<ExchangeHealthControlResultRepository, ExchangeHealthControlResult,
                ExchangeHealthControlResultEntity>(
                _reloadingManager.CurrentValue.Db.Tables.ExchangeHealthControlResult.TableName
            );
        }
    }
}
