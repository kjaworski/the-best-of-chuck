using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TheBestOfChuck.Libraries.AzureTableStorage.Models;
using TheBestOfChuck.Libraries.AzureTableStorage.Repositories;

namespace TheBestOfChuck.Api
{
    public class PullJokes
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IJokeRepository _jokeRepository;

        public PullJokes(IHttpClientFactory clientFactory, IJokeRepository jokeRepository)
        {
            _clientFactory = clientFactory;
            _jokeRepository = jokeRepository;
        }

        [FunctionName(nameof(PullJokes))]
        public async Task Run([TimerTrigger("%PullJokesSchedule%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Starting Azure function: {nameof(PullJokes)}");

            try
            {
                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "3f73dc77dbmshc8497504b4101dbp164fb1jsndd6506616cbf");

                var numberOfJokesToFetch = 10;
                var jokes = new List<JokeEntity>();

                while (numberOfJokesToFetch > 0)
                {
                    var response = await client.GetAsync("jokes/random");
                    response.EnsureSuccessStatusCode();

                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var joke = JsonSerializer.Deserialize<JokeEntity>(responseAsString,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    log.LogInformation(responseAsString);

                    if (joke.Value.Length > 200)
                    {
                        log.LogInformation("Joke skipped, because it was too long.");
                        continue;
                    }

                    if (jokes.Any(j => j.RowKey == joke.RowKey))
                    {
                        continue;
                    }

                    if (_jokeRepository.Get(joke.PartitionKey, joke.RowKey) is not null)
                    {
                        continue;
                    }

                    jokes.Add(joke);
                    numberOfJokesToFetch--;
                }

                await _jokeRepository.AddRangeAsync(jokes);
            }
            catch (Exception exception)
            {
                log.LogError(exception, $"Failed to run {nameof(PullJokes)} function.");

                throw;
            }
        }
    }
}
