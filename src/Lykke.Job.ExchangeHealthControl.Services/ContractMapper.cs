using System;
using AutoMapper;
using Lykke.Job.ExchangeHealthControl.Contract;
using Lykke.Job.ExchangeHealthControl.Core.Domain;

namespace Lykke.Job.ExchangeHealthControl.Services
{
    public class ContractMapper
    {
        private static readonly Lazy<ContractMapper> InstanceHolder = new Lazy<ContractMapper>(() => new ContractMapper());
        private readonly IMapper _mapper;
        
        private ContractMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ExchangeHealthControlResult, ExchangeHealthControlReport>().ReverseMap();
            });

            _mapper = config.CreateMapper();
        }

        public static T Map<T>(object obj)
        {
            return Instance._mapper.Map<T>(obj);
        }

        private static ContractMapper Instance => InstanceHolder.Value;
    }
}
