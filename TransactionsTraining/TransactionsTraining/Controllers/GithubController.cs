using Microsoft.AspNetCore.Mvc;
using PollyTraining.Contracts;
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

        [HttpGet("users/findByUserName/{userName}")]
        public async Task<IActionResult> GetUserByUsername(string userName)
        {
            var user = await _githubService.GetUserByUsernameAsync(userName);
            return user != null ? (IActionResult)Ok(user) : NotFound();
        }

        [HttpGet("users/findByOrgName/{orgName}")]
        public async Task<IActionResult> GetUserFromOrg(string orgName)
        {
            var users = await _githubService.GetUserFromOrgAsync(orgName);
            return users != null ? (IActionResult)Ok(users) : NotFound();
        }
    }
}
