using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Controllers.V1.Requests
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }
              
        public string RefreshToken { get; internal set; }
    }
}
