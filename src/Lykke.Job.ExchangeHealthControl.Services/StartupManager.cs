using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Job.ExchangeHealthControl.Core;
using Lykke.Job.ExchangeHealthControl.Core.Caches;
using Lykke.Job.ExchangeHealthControl.Core.Domain;
using Lykke.Job.ExchangeHealthControl.Core.Repositories;
using Lykke.Job.ExchangeHealthControl.Core.Services;
using Lykke.Job.ExchangeHealthControl.Core.Settings;
using Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings;
using Lykke.SettingsReader;
using MarginTrading.MarketMaker.Contracts;
using MarginTrading.RiskManagement.HedgingService.Contracts.Client;
using MarginTrading.RiskManagement.HedgingService.Contracts.Models;

namespace Lykke.Job.ExchangeHealthControl.Services
{
    // NOTE: Sometimes, startup process which is expressed explicitly is not just better, 
    // but the only way. If this is your case, use this class to manage startup.
    // For example, sometimes some state should be restored before any periodical handler will be started, 
    // or any incoming message will be processed and so on.
    // Do not forget to remove As<IStartable>() and AutoActivate() from DI registartions of services, 
    // which you want to startup explicitly.

    public class StartupManager : IStartupManager
    {
        private readonly IExchangeCache _exchangeCache;

        private readonly IGenericBlobRepository _genericBlobRepository;
        
        private readonly ILog _log;

        public StartupManager(
            IExchangeCache exchangeCache,
            
            IGenericBlobRepository genericBlobRepository,
            
            IReloadingManager<ExchangeHealthControlJobSettings> settings,
            
            ILog log)
        {
            _exchangeCache = exchangeCache;
            
            _genericBlobRepository = genericBlobRepository;
            
            _log = log;
        }

        /// <summary>
        /// Startup logic implementation.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            //read last saved cache
            var savedExchanges = await _genericBlobRepository.ReadAsync<List<Exchange>>(Constants.BlobContainerName,
                Constants.BlobExchangesCache);
            
            //initialize ExchangeCache
            _exchangeCache.Initialize(savedExchanges);
            
            await _log.WriteInfoAsync(nameof(StartupManager), nameof(StartAsync), 
                $"ExchangeCache initialized with data: {string.Join("; ", savedExchanges)}.", DateTime.UtcNow);
        }
    }
}
