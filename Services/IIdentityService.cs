using System;
using System.Threading.Tasks;
using TweetAPI.Domain;

namespace TweetAPI.Controllers.V1
{
    public interface IIdentityService
    {
        Task<AuthenticateResult> RegisterUserAsync(string email, string password);

        Task<AuthenticateResult> LoginAsync(string email, string password);

        Task<AuthenticateResult> RefreshTokenAsync(string Token, string RefreshToken);
        
    }
}