using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Controllers.V1.Responses
{
    public class PostResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
