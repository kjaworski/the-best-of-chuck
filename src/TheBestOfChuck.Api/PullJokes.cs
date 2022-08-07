using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
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
        private readonly ILogger _log;

        public PullJokes(IHttpClientFactory clientFactory, IJokeRepository jokeRepository, ILogger log)
        {
            _clientFactory = clientFactory;
            _jokeRepository = jokeRepository;
            _log = log;
        }

        //TODO: write tests
        [FunctionName(nameof(PullJokes))]
        public async Task Run([TimerTrigger("%PullJokesSchedule%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Starting Azure function: {nameof(PullJokes)}");

            try
            {
                var client = _clientFactory.CreateClient();
                //TODO: move api uri and key to settings file
                client.BaseAddress = new Uri("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "3f73dc77dbmshc8497504b4101dbp164fb1jsndd6506616cbf");

                //TODO: move number of jokes and max length to settings file
                var numberOfJokesToFetch = 10;
                var jokes = new List<JokeEntity>();

                while (numberOfJokesToFetch > 0)
                {
                    var response = await client.GetAsync("jokes/random");
                    response.EnsureSuccessStatusCode();

                    //TODO: move json configuration to Startup class
                    var joke = await response.Content.ReadFromJsonAsync<JokeEntity>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (IsJokeTooLong(joke, maxLength: 200) || IsDuplicateJoke(joke, jokes))
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

        private bool IsDuplicateJoke(JokeEntity joke, IEnumerable<JokeEntity> jokes)
        {
            if (jokes.Any(j => j.RowKey == joke.RowKey))
            {
                _log.LogInformation("Joke was already fetched in current run.");

                return true;
            }

            if (_jokeRepository.Get(joke.PartitionKey, joke.RowKey) is not null)
            {
                _log.LogInformation("Joke already found in database.");

                return true;
            }

            return false;
        }

        private bool IsJokeTooLong(JokeEntity joke, int maxLength)
        {
            _log.LogInformation("Joke is too long.");

            return joke.Value.Length > maxLength;
        }
    }
}
