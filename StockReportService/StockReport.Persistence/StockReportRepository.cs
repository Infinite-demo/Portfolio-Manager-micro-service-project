using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbClient;
using StockReport.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockReport.Persistence
{
    public interface IStockReportRepository
    {
        Task UpdateStockReport(string userName, string stockName, int quantity);
        Task<Result<Domain.StockReport>> Get(string userName, string stockName);
        Task<FilteredResult<Domain.StockReport>> Get(int skip = 0, int pageLimit = 0);
    }

    public class StockReportRepository : MongoDbRepository<Domain.StockReport>, IStockReportRepository
    {
        public StockReportRepository(IDatabaseContext databaseContext) : base(databaseContext) { }
        public async Task UpdateStockReport(string userName, string stockName, int quantity)
        {
            var record = await Get(userName, stockName);
            if (record.IsSuccess == false)
            {
                await collection.InsertOneAsync(new Domain.StockReport
                {
                    Quantity = quantity,
                    UserName = userName,
                    StockName = stockName
                });
            }
            else
            {
                record.Value.Quantity += quantity;
                var filter = Builders<Domain.StockReport>.Filter.Eq(x => x.UserName, userName);
                var update = Builders<Domain.StockReport>.Update.Set(x => x.Quantity, record.Value.Quantity);
                await collection.UpdateOneAsync(filter, update);
            }
        }

        public async Task<Result<Domain.StockReport>> Get(string userName, string stockName)
        {
            try
            {
                var result = await collection.AsQueryable().FirstOrDefaultAsync(x => x.UserName == userName && x.StockName.ToLower() == stockName.ToLower());
                if (result == null) return await Task.FromResult(Result<Domain.StockReport>.Fail("No record found"));
                else return await Task.FromResult(Result<Domain.StockReport>.Success(result));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Result<Domain.StockReport>.Fail(ex.ToString()));
            }
        }

        public async Task<FilteredResult<Domain.StockReport>> Get(int skip = 0, int pageLimit = 0)
        {
            var totalCount = await collection.AsQueryable().CountAsync();
            IMongoQueryable<Domain.StockReport> query = collection.AsQueryable();
            if (skip > 0) query = query.Skip(skip);
            if (pageLimit > 0) query = query.Take(pageLimit);
            var result = await query.ToListAsync();
            return new FilteredResult<Domain.StockReport>
            {
                TotalCount = totalCount,
                Result = result
            };
        }
    }
}
