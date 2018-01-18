using System.Threading.Tasks;

namespace Lykke.Job.ExchangeHealthControl.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
