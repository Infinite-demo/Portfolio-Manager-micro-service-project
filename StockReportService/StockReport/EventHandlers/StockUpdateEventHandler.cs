using EventBus.Contracts;
using Service.Events;
using StockReport.Service;
using System.Threading.Tasks;

namespace StockReport.EventHandlers
{
    public class StockUpdateEventHandler : IIntegrationEventHandler<StockUpdateEvent>
    {
        public IStockReportService _service;
        public StockUpdateEventHandler(IStockReportService stockReportService)
        {
            _service = stockReportService;
        }


        public async Task HandleAsync(StockUpdateEvent @event)
        {
            _service.UpdateStockReport(@event);
        }
    }
}
