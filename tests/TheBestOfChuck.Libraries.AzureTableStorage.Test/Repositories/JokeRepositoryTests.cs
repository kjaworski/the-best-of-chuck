using Azure.Data.Tables;
using TheBestOfChuck.Libraries.AzureTableStorage.Repositories;

namespace TheBestOfChuck.Libraries.AzureTableStorage.Test.Repositories
{
    public class JokeRepositoryTests
    {
        private IJokeRepository _sut;

        [SetUp]
        public void Setup()
        {
            //TODO: mock service client
            _sut = new JokeRepository(new TableServiceClient("UseDevelopmentStorage=true"));
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}