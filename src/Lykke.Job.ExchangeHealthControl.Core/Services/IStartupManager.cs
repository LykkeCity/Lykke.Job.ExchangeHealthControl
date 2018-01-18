using System.Threading.Tasks;

namespace Lykke.Job.ExchangeHealthControl.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}
