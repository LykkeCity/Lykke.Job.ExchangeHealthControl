using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.ExchangeHealthControl.Core.Services;

namespace Lykke.Job.ExchangeHealthControl.PeriodicalHandlers
{
    public class ExchangeHealthControlHandler : TimerPeriod
    {
        private string ExchangeName { get; }

        private readonly IExchangeHealthControlService _ExchangeHealthControlService;

        private readonly int _pollingPeriodMilliseconds;

        protected ExchangeHealthControlHandler(
            IExchangeHealthControlService ExchangeHealthControlService,
            ILog log,
            string exchangeName,
            int pollingPeriodMilliseconds)
            : base(nameof(ExchangeHealthControlHandler), pollingPeriodMilliseconds, log)
        {
            _ExchangeHealthControlService = ExchangeHealthControlService;

            ExchangeName = exchangeName;

            _pollingPeriodMilliseconds = pollingPeriodMilliseconds;
        }

        public override async Task Execute()
        {
            await _ExchangeHealthControlService.Poll(ExchangeName, TimeSpan.FromMilliseconds(_pollingPeriodMilliseconds));
        }
    }
}
