using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDbClient;
using System;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.Domain;

namespace UserManagement.Persistence
{
    public interface IUserRepository
    {
        Task<User> Add(User user);
        Task<Result<User>> Get(string id);
        Task<FilteredResult> Get(int skip = 0, int pageLimit = 0);
    }

    public class UserRepository : MongoDbRepository<User>, IUserRepository
    {
        public UserRepository(IDatabaseContext databaseContext) : base(databaseContext) { }
        public async Task<User> Add(User user)
        {
            await collection.InsertOneAsync(user);
            return user;
        }

        public async Task<Result<User>> Get(string id)
        {
            try
            {
                var result = await collection.AsQueryable().FirstOrDefaultAsync(x => x.Id == id);
                if (result == null) return await Task.FromResult(Result<User>.Fail("No record found"));
                else return await Task.FromResult(Result<User>.Success(result));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Result<User>.Fail(ex.ToString()));
            }
        }

        public async Task<FilteredResult> Get(int skip = 0, int pageLimit = 0)
        {
            var totalCount = await collection.AsQueryable().CountAsync();
            IMongoQueryable<User> query = collection.AsQueryable();
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
