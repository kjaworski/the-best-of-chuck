using Azure;
using Azure.Data.Tables;

namespace TheBestOfChuck.Libraries.AzureTableStorage.Models
{
    public class JokeEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string? Value { get; set; }
    }
}
