using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Domain
{
    public class AuthenticateResult
    {
        public string Token { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> ErrorMessages { get; set; }
    }

}
