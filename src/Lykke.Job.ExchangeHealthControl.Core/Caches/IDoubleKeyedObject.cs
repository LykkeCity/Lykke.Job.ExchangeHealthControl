namespace Lykke.Job.ExchangeHealthControl.Core.Caches
{
    public interface IDoubleKeyedObject
    {
        string GetPartitionKey { get; }
        string GetRowKey { get; }
    }
}
