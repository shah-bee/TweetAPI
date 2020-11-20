using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TweetAPI.Extensions
{
    public static class UserExtension
    {
        public static string GetUserId(this HttpContext httpContext) {

            var userId = httpContext.User.Claims.SingleOrDefault(claim => claim.Type == "Id").Value;

            if (userId == null) {
                return null;
            }

            return userId;
        }
    
    }
}
