using System;
using Lykke.Job.ExchangeHealthControl.Core.Domain;
using Lykke.Job.ExchangeHealthControl.Core.Entities;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories.Entity
{
    public class ExchangeHealthControlResultEntity 
        : MappableTableEntity<ExchangeHealthControlResult, ExchangeHealthControlResultEntity>, 
            IExchangeHealthControlResultEntity
    {
        public string ExchangeName { get; set; }
        public int RequestDurationMilliseconds { get; set; }
        public string Type { get; set; }
        public Exception Exception { get; set; }
        public bool IsSuccessful { get; set; }

        protected override string GetPartitionKey() => ExchangeName;

        protected override string GetRowKey() => $"{DateTime.UtcNow:s}";
        
        
    }
}
