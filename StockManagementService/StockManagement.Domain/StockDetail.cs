using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockManagement.Domain
{
    public class StockDetail
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string StockName { get; set; }
        public string UserName { get; set; }
        public int Quantity { get; set; }
    }
}
