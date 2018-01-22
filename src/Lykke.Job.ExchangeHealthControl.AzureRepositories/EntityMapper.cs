using System;
using AutoMapper;
using Lykke.Job.ExchangeHealthControl.AzureRepositories.Entity;
using Lykke.Job.ExchangeHealthControl.Core.Domain;

namespace Lykke.Job.ExchangeHealthControl.AzureRepositories
{
    public class EntityMapper
    {
        private static readonly Lazy<EntityMapper> InstanceHolder = new Lazy<EntityMapper>(() => new EntityMapper());
        private readonly IMapper _mapper;

        private EntityMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExchangeHealthControlResultEntity, ExchangeHealthControlResult>().ReverseMap();
                /*cfg.CreateMap<QuoteSnapshotEntity, QuoteSnapshot>().ReverseMap();
                cfg.CreateMap<ExternalPositionHistoryEntity, ExecutedTrade>().ReverseMap();
                cfg.CreateMap<ClientAccountEntity, ClientAccount>().ReverseMap();
                cfg.CreateMap<OrderModelEntity, OrderModel>().ReverseMap();
                cfg.CreateMap<OverallPositionMonitorEntity, OverallPositionMonitor>().ReverseMap();
                cfg.CreateMap<AssetAggregatedHedgingEntity, AssetAggregatedHedgingModel>().ReverseMap();
                cfg.CreateMap<ExternalPositionEntity, ExternalPosition>().ReverseMap();
                cfg.CreateMap<MarginControlResultEntity, MarginControlResult>().ReverseMap(); //
                cfg.CreateMap<TradingPositionEntity, TradingPosition>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TakerPositionId))
                    .ForMember(dest => dest.TraderCounterPartyId, opt => opt.MapFrom(src => src.TakerCounterpartyId))
                    .ForMember(dest => dest.TraderAccountId, opt => opt.MapFrom(src => src.TakerAccountId))
                    .ForMember(dest => dest.TraderAccountAssetId, opt => opt.MapFrom(src => src.TakerAccountAssetId))
                    .ReverseMap();*/
            });

            _mapper = config.CreateMapper();
        }

        public static T Map<T>(object obj)
        {
            return Instance._mapper.Map<T>(obj);
        }

        private static EntityMapper Instance => InstanceHolder.Value;
    }
}
