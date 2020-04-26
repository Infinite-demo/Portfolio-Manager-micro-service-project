using AutoMapper;
using Service.Events;
using StockReport.Domain;
using StockReport.Model;
using StockReport.Persistence;
using System.Threading.Tasks;

namespace StockReport.Service
{
    public interface IStockReportService
    {
        Task<FilteredResult<StockReportDto>> Get(int skip = 0, int pageLimit = 0);
        void UpdateStockReport(StockUpdateEvent @event);
    }

    public class StockReportService : IStockReportService
    {
        private readonly IMapper _mapper;
        public readonly IStockReportRepository _repository;

        public StockReportService(IMapper mapper, IStockReportRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<FilteredResult<StockReportDto>> Get(int skip = 0, int pageLimit = 0)
        {
            var result = await _repository.Get(skip, pageLimit);
            return _mapper.Map<FilteredResult<StockReportDto>>(result);
        }

        public void UpdateStockReport(StockUpdateEvent @event)
        {
            _repository.UpdateStockReport(@event.UserName, @event.StockName, @event.Quantity);
        }
    }
}
