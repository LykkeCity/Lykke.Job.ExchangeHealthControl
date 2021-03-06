﻿using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories
{
    public abstract class MappableTableEntity<TD, TE> : TableEntity
        where TD : class
        where TE : MappableTableEntity<TD, TE>, new()
    {
        public MappableTableEntity() : base()
        {
        }

        protected abstract string GetPartitionKey();

        protected abstract string GetRowKey();

        public static TE Create(TD domainObject)
        {
            var entity = EntityMapper.Map<TE>(domainObject);

            entity.PartitionKey = entity.GetPartitionKey();
            entity.RowKey = entity.GetRowKey();

            return entity;
        }

        public static TD Restore(TE entity)
        {
            return EntityMapper.Map<TD>(entity);
        }
    }
}
