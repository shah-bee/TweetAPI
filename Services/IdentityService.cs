using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TweetAPI.Controllers.V1;
using TweetAPI.Domain;
using TweetAPI.Installer;

namespace TweetAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly JWTSettings jwtSettings;

        public IdentityService(UserManager<IdentityUser> userManager, JWTSettings jwtSettings)
        {
            this.userManager = userManager;
            this.jwtSettings = jwtSettings;
        }

        public async Task<AuthenticateResult> LoginUserAsync(string email, string password)
        {
            var userExists = await userManager.FindByNameAsync(email);

            if (userExists == null)
            {
                var result = new AuthenticateResult
                {
                    Success = false,
                    ErrorMessages = new string[] { "User doesnt exist" }
                };
                return result;
            }

            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };

            var createdUser = await userManager.CheckPasswordAsync(user, password);
            if (!createdUser)
            {
               return new AuthenticateResult
                {
                    Success = false,
                    ErrorMessages = new string[] { "User/password is wrong" }
                };
            }

            return GenerateAuthenticationResult(user);
        }

        public async Task<AuthenticateResult> RegisterUserAsync(string email, string password)
        {
            var userExists = await userManager.FindByNameAsync(email);

            if (userExists != null)
            {
                var result = new AuthenticateResult
                {
                    Success = false,
                    ErrorMessages = new string[] { "User already exists with this Email Address" }
                };

                return result;
            }

            var user = new IdentityUser
            {

                UserName = email,
                Email = email
            };

            var createdUser = await userManager.CreateAsync(user, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticateResult
                {
                    ErrorMessages = createdUser.Errors.Select(x => x.Description)
                };
            }

           return GenerateAuthenticationResult(user);
        }

        private AuthenticateResult GenerateAuthenticationResult(IdentityUser user) {

            var secret = Encoding.ASCII.GetBytes(this.jwtSettings.Secret);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                     new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                     new Claim(JwtRegisteredClaimNames.Email, user.Email),
                     new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                     new Claim("id", user.Id)
                    }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthenticateResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token)
            };

        }
    }
}
