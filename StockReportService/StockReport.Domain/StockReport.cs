using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockReport.Domain
{
    public class StockReport
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string StockName { get; set; }
        public string UserName { get; set; }
        public int Quantity { get; set; }
    }
}
