using PollyTraining.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PollyTraining.Services
{
    public interface IGithubService
    {
        Task<GithubUser> GetUserByUsernameAsync(string username);

        Task<List<GithubUser>> GetUserFromOrgAsync(string orgName);

    }
}