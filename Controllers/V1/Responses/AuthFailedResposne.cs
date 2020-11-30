using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace TweetAPI.Controllers.V1
{
    internal class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}