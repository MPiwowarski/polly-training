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
        private readonly HttpClient _httpClient;
        private static readonly Random _random = new Random();
        private readonly AsyncRetryPolicy _retryPolicy;

        public GithubService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("github");
            _retryPolicy = Policy.Handle<HttpRequestException>(exception =>
            {
               // example how to handle exceptions with polly
               return exception.Message != "Exception test";

            }).WaitAndRetryAsync(_maxRetries, times => TimeSpan.FromSeconds(1));
        }

        public async Task<GithubUser> GetUserByUsernameAsync(string username)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                // added for polly tests
                if (_random.Next(1, 3) == 1)
                    throw new HttpRequestException("This is a fake request exception");

                var result = await _httpClient.GetAsync($"/users/{username}");
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                var resultString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GithubUser>(resultString);
            });
        }

        public async Task<List<GithubUser>> GetUserFromOrgAsync(string orgName)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var result = await _httpClient.GetAsync($"/orgs/{orgName}");
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                var resultString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<GithubUser>>(resultString);
            });
        }
    }

}
