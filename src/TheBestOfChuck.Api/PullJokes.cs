using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TheBestOfChuck.Api
{
    public class PullJokes
    {
        private readonly IHttpClientFactory _clientFactory;

        public PullJokes(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [FunctionName(nameof(PullJokes))]
        public async Task Run([TimerTrigger("%PullJokesSchedule%")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            try
            {
                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri("https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "3f73dc77dbmshc8497504b4101dbp164fb1jsndd6506616cbf");

                var numberOfJokesToFetch = 10;
                var jokes = new List<Joke>();

                while (numberOfJokesToFetch > 0)
                {
                    var response = await client.GetAsync("jokes/random");
                    response.EnsureSuccessStatusCode();

                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var joke = JsonSerializer.Deserialize<Joke>(responseAsString,
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

                    jokes.Add(joke);
                    numberOfJokesToFetch--;
                }
            }
            catch (Exception exception)
            {
                log.LogError(exception, $"Failed to run {nameof(PullJokes)} function.");

                throw;
            }
        }
    }
}
