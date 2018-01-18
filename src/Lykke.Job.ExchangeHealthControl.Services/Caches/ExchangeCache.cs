using System.Collections.Generic;
using System.Linq;
using Lykke.Job.ExchangeHealthControl.Core.Caches;
using Lykke.Job.ExchangeHealthControl.Core.Domain;
using MoreLinq;

namespace Lykke.Job.ExchangeHealthControl.Services.Caches
{
    public class ExchangeCache : GenericDictionaryCache<Exchange>, IExchangeCache
    {
    }
}
