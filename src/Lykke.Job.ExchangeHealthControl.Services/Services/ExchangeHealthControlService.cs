using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Job.ExchangeHealthControl.Contract;
using Lykke.Job.ExchangeHealthControl.Contract.Enums;
using Lykke.Job.ExchangeHealthControl.Core;
using Lykke.Job.ExchangeHealthControl.Core.Caches;
using Lykke.Job.ExchangeHealthControl.Core.Domain;
using Lykke.Job.ExchangeHealthControl.Core.Services;
using Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings;
using Lykke.Job.ExchangeHealthControl.Services.Caches;
using Lykke.Service.ExchangeConnector.Client;
using Lykke.SettingsReader;
using MoreLinq;

namespace Lykke.Job.ExchangeHealthControl.Services.Services
{
    public class ExchangeHealthControlService : IExchangeHealthControlService
    {
        private readonly IExchangeCache _exchangeCache;

        private readonly IExchangeConnectorService _exchangeConnectorService;

        private readonly IRabbitMqPublisher<ExchangeHealthControlReport> _exchangeHealthControlReportPublisher;
        
        private readonly IReloadingManager<ExchangeHealthControlJobSettings> _settings;

        private readonly ILog _log;
        
        public ExchangeHealthControlService(
            IExchangeCache exchangeCache,
            
            IExchangeConnectorService exchangeConnectorService,
            
            IRabbitMqPublisher<ExchangeHealthControlReport> exchangeHealthControlReportPublisher,
            
            IReloadingManager<ExchangeHealthControlJobSettings> settings,
            
            ILog log)
        {
            _exchangeCache = exchangeCache;

            _exchangeConnectorService = exchangeConnectorService;

            _exchangeHealthControlReportPublisher = exchangeHealthControlReportPublisher;

            _settings = settings;

            _log = log;
        }
        
        public async Task Poll(string exchangeName, TimeSpan timeout)
        {
            var tokenSource = new CancellationTokenSource(timeout);
            var watch = new Stopwatch();
            var requestDuration = 0;
            Exception exception = null;
            ExchangeHealthControlReportType type;
            
            //request positions state
            try
            {
                watch.Start();
                
                var requestResult = await _exchangeConnectorService.GetOpenedPositionAsync(exchangeName, tokenSource.Token);
                
                requestDuration = (int)watch.ElapsedMilliseconds;
                type = requestDuration == null
                    ? ExchangeHealthControlReportType.NoPositionData
                    : ExchangeHealthControlReportType.Ok;
            }
            catch (Exception ex)
            {
                exception = ex;
                type = ExchangeHealthControlReportType.ExceptionRased;
                await _log.WriteErrorAsync(nameof(ExchangeHealthControlService), nameof(Poll), ex, DateTime.UtcNow);
            }
            
            //push result to the rabbit (to be consumed by Hedging)
            await _exchangeHealthControlReportPublisher.Publish(
                new ExchangeHealthControlReport(
                    exchangeName, 
                    requestDuration, 
                    type.ToString(), 
                    exception, 
                    type == ExchangeHealthControlReportType.Ok));
        }
    }
}
