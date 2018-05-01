using System;
using System.Collections.Concurrent;
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
using Lykke.Job.ExchangeHealthControl.Core.Repositories;
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

        private readonly IExchangeHealthControlResultRepository _exchangeHealthControlResultRepository;
        
        private readonly IReloadingManager<ExchangeHealthControlJobSettings> _settings;

        private readonly ILog _log;

        private readonly ConcurrentDictionary<string, DateTime> _warningCache = new ConcurrentDictionary<string, DateTime>();

        private readonly int _failMessageThrottlingPeriodSec;
        
        public ExchangeHealthControlService(
            IExchangeCache exchangeCache,
            
            IExchangeConnectorService exchangeConnectorService,
            
            IRabbitMqPublisher<ExchangeHealthControlReport> exchangeHealthControlReportPublisher,
            
            IExchangeHealthControlResultRepository exchangeHealthControlResultRepository,
            
            IReloadingManager<ExchangeHealthControlJobSettings> settings,
            
            ILog log)
        {
            _exchangeCache = exchangeCache;

            _exchangeConnectorService = exchangeConnectorService;

            _exchangeHealthControlReportPublisher = exchangeHealthControlReportPublisher;

            _exchangeHealthControlResultRepository = exchangeHealthControlResultRepository;

            _settings = settings;

            _log = log;

            _failMessageThrottlingPeriodSec = _settings.CurrentValue.FailMessageThrottlingPeriodSeconds;
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

                var requestResult =
                    await _exchangeConnectorService.GetOpenedPositionAsync(exchangeName, tokenSource.Token);

                requestDuration = (int) watch.ElapsedMilliseconds;
                type = requestResult == null
                    ? ExchangeHealthControlReportType.NoPositionData
                    : ExchangeHealthControlReportType.Ok;
            }
            catch (OperationCanceledException ex)
            {
                exception = ex;
                type = ExchangeHealthControlReportType.ExceptionRased;
            }
            catch (Exception ex)
            {
                exception = ex;
                type = ExchangeHealthControlReportType.ExceptionRased;

                if (!_warningCache.TryGetValue(exchangeName, out var lastWarning) ||
                    DateTime.UtcNow.Subtract(lastWarning).TotalSeconds > _failMessageThrottlingPeriodSec)
                {
                    _warningCache.AddOrUpdate(exchangeName, DateTime.UtcNow, (e, t) => DateTime.UtcNow);
                    await _log.WriteWarningAsync(nameof(ExchangeHealthControlService), nameof(Poll),
                        $"Exception occured while polling {exchangeName}.", ex, DateTime.UtcNow);
                }
            }

            var report = new ExchangeHealthControlResult(
                exchangeName, 
                requestDuration, 
                type.ToString(), 
                exception, 
                type == ExchangeHealthControlReportType.Ok);
            
            //push result to the rabbit (to be consumed by Hedging)
            await _exchangeHealthControlReportPublisher.Publish(ContractMapper.Map<ExchangeHealthControlReport>(report));

            //write statistic to table
            await _exchangeHealthControlResultRepository.InsertOrUpdateAsync(report);
        }
    }
}
