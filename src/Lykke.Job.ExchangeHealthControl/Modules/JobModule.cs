using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.ExchangeHealthControl.AzureRepositories;
using Lykke.Job.ExchangeHealthControl.Contract;
using Lykke.Job.ExchangeHealthControl.Core.Caches;
using Lykke.Job.ExchangeHealthControl.Core.Repositories;
using Lykke.Job.ExchangeHealthControl.Core.Services;
using Lykke.Job.ExchangeHealthControl.Core.Settings.JobSettings;
using Lykke.Job.ExchangeHealthControl.PeriodicalHandlers;
using Lykke.Job.ExchangeHealthControl.RabbitPublishers;
using Lykke.Job.ExchangeHealthControl.RabbitSubscribers;
using Lykke.Job.ExchangeHealthControl.Services;
using Lykke.Job.ExchangeHealthControl.Services.Caches;
using Lykke.Job.ExchangeHealthControl.Services.Services;
using Lykke.Service.ExchangeConnector.Client;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest;
using MoreLinq;

namespace Lykke.Job.ExchangeHealthControl.Modules
{
    public class JobModule : Module
    {
        private readonly IReloadingManager<ExchangeHealthControlJobSettings> _settings;
        private readonly bool _isDevelopment;

        private readonly ILog _log;

        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(IReloadingManager<ExchangeHealthControlJobSettings> settings,
            bool isDevelopment, 
            ILog log)
        {
            _settings = settings;
            _isDevelopment = isDevelopment;
            _log = log;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterInstance(_settings)
                .As<IReloadingManager<ExchangeHealthControlJobSettings>>()
                .SingleInstance();
            
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            
            RegisterPeriodicalHandlers(builder);
            
            RegisterRabbitMqSubscribers(builder);

            RegisterRabbitMqPublishers(builder);

            builder.Register<IGenericBlobRepository>(ctx =>
                    new GenericBlobRepository(_settings.Nested(x => x.Db.BlobConnString)))
                .SingleInstance();

            builder.RegisterType<ExchangeHealthControlService>()
                .As<IExchangeHealthControlService>()
                .SingleInstance();

            builder.RegisterType<ExchangeCache>()
                .As<IExchangeCache>()
                .SingleInstance();
            
            builder.RegisterType<ExchangeConnectorService>()
                .As<IExchangeConnectorService>()
                .WithParameter("settings",new ExchangeConnectorServiceSettings
                {
                    ServiceUrl = _settings.CurrentValue.Services.ExchangeConnectorService.Url,
                    ApiKey = _settings.CurrentValue.Services.ExchangeConnectorService.ApiKey
                })
                .SingleInstance();
            
            builder.Populate(_services);
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            _settings.CurrentValue.ExchangePolling.ExchangeList.ForEach(exchange =>
            {
                builder.RegisterType<ExchangeHealthControlHandler>()
                    .WithParameter(TypedParameter.From(_settings.CurrentValue.ExchangePolling.PollingPeriodMilliseconds))
                    .WithParameter(TypedParameter.From(exchange))
                    .SingleInstance();
            });
            
            builder.RegisterType<DataSavingHandler>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.DataSavingPeriodMilliseconds))
                .SingleInstance();
        }
        
        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            
        }

        private void RegisterRabbitMqPublishers(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitMqPublisher<ExchangeHealthControlReport>>()
                .As<IRabbitMqPublisher<ExchangeHealthControlReport> >()
                .SingleInstance()
                .WithParameters(new[]
                {
                    new NamedParameter("connectionString", _settings.CurrentValue.Rabbit.ExchangeConnectorHealthControl.ConnectionString),
                    new NamedParameter("exchangeName", _settings.CurrentValue.Rabbit.ExchangeConnectorHealthControl.ExchangeName),
                    new NamedParameter("enabled", true),
                    new NamedParameter("log", _log)
                });
        }
    }
}
