using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using TheBestOfChuck.Libraries.AzureTableStorage.Repositories;

namespace TheBestOfChuck.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //TODO: inject azure client with connection string
            builder.Services.AddTransient<IJokeRepository, JokeRepository>();
        }
    }
}
