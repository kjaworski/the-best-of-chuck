using Azure.Data.Tables;
using TheBestOfChuck.Libraries.AzureTableStorage.Models;

namespace TheBestOfChuck.Libraries.AzureTableStorage.Repositories
{
    public class JokeRepository : BaseRepository<JokeEntity>, IJokeRepository
    {
        //TODO: write tests
        public JokeRepository(TableServiceClient tableServiceClient) : base(tableServiceClient)
        {
        }

        protected override string TableName => "Joke";
    }
}
