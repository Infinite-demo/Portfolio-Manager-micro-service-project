using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbClient;
using StockManagement.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockManagement.Persistence
{
    public interface IStockDetailRepository
    {
        Task<StockDetail> Add(StockDetail user);
        Task<Result<StockDetail>> Get(string id);
        Task<FilteredResult> Get(int skip = 0, int pageLimit = 0);
    }

    public class StockDetailRepository : MongoDbRepository<StockDetail>, IStockDetailRepository
    {
        public StockDetailRepository(IDatabaseContext databaseContext) : base(databaseContext) { }
        public async Task<StockDetail> Add(StockDetail user)
        {
            await collection.InsertOneAsync(user);
            return user;
        }

        public async Task<Result<StockDetail>> Get(string id)
        {
            try
            {
                var result = await collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
                if (result == null) return await Task.FromResult(Result<StockDetail>.Fail("No record found"));
                else return await Task.FromResult(Result<StockDetail>.Success(result));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Result<StockDetail>.Fail(ex.ToString()));
            }
        }

        public async Task<FilteredResult> Get(int skip = 0, int pageLimit = 0)
        {
            var totalCount = await collection.AsQueryable().CountAsync();
            IMongoQueryable<StockDetail> query = collection.AsQueryable();
            if (skip > 0) query = query.Skip(skip);
            if (pageLimit > 0) query = query.Take(pageLimit);
            var result = await query.ToListAsync();
            return new FilteredResult
            {
                TotalCount = totalCount,
                Result = result
            };
        }
    }
}
