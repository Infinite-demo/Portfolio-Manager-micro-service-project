using EventBus.Contracts;

namespace Service.Events
{
    public class StockUpdateEvent : IIntegrationEvent
    {
        public StockUpdateEvent()
        {
            EventName = GetType().Name;
        }

        public string EventName { get; set; }
        public Context Context { get; set; }
        public string StockName { get; set; }
        public string UserName { get; set; }
        public int Quantity { get; set; }
    }
}
