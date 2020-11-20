using System.Threading.Tasks;
using TweetAPI.Domain;

namespace TweetAPI.Controllers.V1
{
    public interface IIdentityService
    {
        Task<AuthenticateResult> RegisterUserAsync(string Email, string Password);

        Task<AuthenticateResult> LoginUserAsync(string Email, string Password);
        
    }
}