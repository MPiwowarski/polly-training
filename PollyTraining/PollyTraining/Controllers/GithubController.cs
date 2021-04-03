using Microsoft.AspNetCore.Mvc;
using PollyTraining.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PollyTraining.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GithubController : ControllerBase
    {
        private readonly IGithubService _githubService;

        public GithubController(IGithubService githubService)
        {
            _githubService = githubService;
        }

        [HttpGet("users/{userName}")]
        public async Task<GithubUser> Get(string userName)
        {
            return await _githubService.GetUserByUsernameAsync(userName);
        }

        
    }
}
