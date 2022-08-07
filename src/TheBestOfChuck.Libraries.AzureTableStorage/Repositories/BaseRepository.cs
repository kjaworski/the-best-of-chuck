using Azure.Data.Tables;

namespace TheBestOfChuck.Libraries.AzureTableStorage.Repositories
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class, ITableEntity, new()
    {
        private readonly TableClient _tableClient;

        protected abstract string TableName { get; }

        protected BaseRepository(TableServiceClient tableServiceClient)
        {
            _tableClient = tableServiceClient.GetTableClient(TableName);
            _tableClient.CreateIfNotExists();
        }

        //TODO: error handling
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                var transactions = new List<TableTransactionAction>();
                transactions.AddRange(
                    entities.Select(e =>
                        new TableTransactionAction(TableTransactionActionType.Add, e)));

                await _tableClient.SubmitTransactionAsync(transactions);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<TEntity> Get(string partitionKey, string rowKey)
        {
            try
            {
                var result = await _tableClient.GetEntityAsync<TEntity>(partitionKey, rowKey);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
