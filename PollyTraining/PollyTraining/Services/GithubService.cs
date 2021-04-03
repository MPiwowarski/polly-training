using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using PollyTraining.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PollyTraining.Services
{
    public class GithubService : IGithubService
    {
        private const int _maxRetries = 3;
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly Random _random = new Random();
        private readonly AsyncRetryPolicy<GithubUser> _retryPolicy;

        public GithubService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _retryPolicy = Policy<GithubUser>.Handle<HttpRequestException>().RetryAsync(_maxRetries);
        }

        public async Task<GithubUser> GetUserByUsernameAsync(string username)
        {
            var client = _httpClientFactory.CreateClient("github");

            return await _retryPolicy.ExecuteAsync(async () =>
            {
                // added for polly tests
                if (_random.Next(1, 3) == 1)
                    throw new HttpRequestException("This is a fake request exception");

                var result = await client.GetAsync($"/users/{username}");
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                var resultString = await result.Content.ReadAsStringAsync();
                return JObject.Parse(resultString).ToObject<GithubUser>();
            });
        }
    }

}
