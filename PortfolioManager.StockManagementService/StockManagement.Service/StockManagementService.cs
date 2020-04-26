using AutoMapper;
using EventBus.Contracts;
using Service.Events;
using StockManagement.Domain;
using StockManagement.Model;
using StockManagement.Persistence;
using System.Threading.Tasks;

namespace StockManagement.Service
{
    public interface IStockManagementService
    {
        Task<StockDetail> Add(StockDetailDto userDto);
        Task<Result<StockDetail>> Get(string id);
        Task<FilteredResult> Get(int skip = 0, int pageLimit = 0);
    }

    public class StockManagementService : IStockManagementService
    {
        private readonly IEventBus _evenBus;
        private readonly IMapper _mapper;
        public readonly IStockDetailRepository _repository;

        public StockManagementService(IMapper mapper, IStockDetailRepository repository, IEventBus evenBus)
        {
            _mapper = mapper;
            _evenBus = evenBus;
            _repository = repository;
        }

        public async Task<StockDetail> Add(StockDetailDto userDto)
        {
            await Publish(userDto);
            return await _repository.Add(_mapper.Map<StockDetail>(userDto));
        }

        public Task<Result<StockDetail>> Get(string id)
        {
            return _repository.Get(id);
        }

        public async Task<FilteredResult> Get(int skip = 0, int pageLimit = 0)
        {
            return await _repository.Get(skip, pageLimit);
        }

        private async Task Publish(StockDetailDto userDto)
        {
            var @event = new StockUpdateEvent
            {
                Context = new Context(),
                StockName = userDto.StockName,
                Quantity = userDto.Quantity,
                UserName = userDto.UserName
            };

            await _evenBus.Publish<StockUpdateEvent>(@event);
        }
    }
}
