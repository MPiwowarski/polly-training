using System.Threading.Tasks;

namespace PollyTraining.Services
{
    public interface IGithubService
    {
        Task<GithubUser> GetUserByUsernameAsync(string username);
    }
}