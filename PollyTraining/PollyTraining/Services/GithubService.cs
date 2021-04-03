using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public GithubService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<GithubUser> GetUserByUsernameAsync(string username)
        {

            var client = _httpClientFactory.CreateClient("github");
            var retriesLeft = _maxRetries;

            GithubUser githubUser = null;
            while (retriesLeft > 0)
            {
                try
                {
                    // added for polly tests
                    //if (_random.Next(1, 3) == 1)
                    //    throw new HttpRequestException("This is a fake request exception");

                    var result = await client.GetAsync($"/users/{username}");
                    if (result.StatusCode == HttpStatusCode.NotFound)
                    {
                        break;
                    }

                    var resultString = await result.Content.ReadAsStringAsync();
                    githubUser = JObject.Parse(resultString).ToObject<GithubUser>();
                    break;

                }
                catch (HttpRequestException e)
                {
                    retriesLeft--;
                    if (retriesLeft == 0)
                    {
                        throw;
                    }
                }
                catch(Exception e)
                {
                    retriesLeft--;
                    if (retriesLeft == 0)
                    {
                        throw;
                    }
                }
            }

            return githubUser;
        }
    }

}
