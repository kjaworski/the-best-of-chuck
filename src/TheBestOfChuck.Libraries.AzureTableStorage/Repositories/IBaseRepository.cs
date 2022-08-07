using Azure.Data.Tables;

namespace TheBestOfChuck.Libraries.AzureTableStorage.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : ITableEntity, new()
    {
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task<TEntity> Get(string partitionKey, string rowKey);
    }
}
