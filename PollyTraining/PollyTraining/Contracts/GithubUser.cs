using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PollyTraining.Contracts
{
    public class GithubUser
    {
        public string Login { get; set; }

        public int Id { get; set; }

        public string AvatarUrl { get; set; }
    }
}
