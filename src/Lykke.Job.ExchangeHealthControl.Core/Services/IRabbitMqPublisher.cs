using System.Threading.Tasks;

namespace Lykke.Job.ExchangeHealthControl.Core.Services
{
    public interface IRabbitMqPublisher<in T>
    {
        Task Publish(T message);
    }
}
