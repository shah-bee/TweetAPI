using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Controllers.V1.Requests
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public IEnumerable<string> Messages { get; set; }
    }
}
