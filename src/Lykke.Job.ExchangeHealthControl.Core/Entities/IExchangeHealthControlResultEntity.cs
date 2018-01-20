using System;

namespace Lykke.Job.ExchangeHealthControl.Core.Entities
{
    public interface IExchangeHealthControlResultEntity
    {
        string ExchangeName { get; }
        int RequestDurationMilliseconds { get; }
        string Type { get; }
        Exception Exception { get; }
        bool IsSuccessful { get; }
    }
}
