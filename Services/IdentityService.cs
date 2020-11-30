using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TweetAPI.Controllers.V1;
using TweetAPI.Data;
using TweetAPI.Domain;
using TweetAPI.Installer;

namespace TweetAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly JWTSettings jwtSettings;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly ApplicationDataContext dataContext;

        public IdentityService(UserManager<IdentityUser> userManager, JWTSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters, ApplicationDataContext dataContext)
        {
            this.userManager = userManager;
            this.jwtSettings = jwtSettings;
            this.tokenValidationParameters = tokenValidationParameters;
            this.dataContext = dataContext;
        }

        public async Task<AuthenticateResult> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                var result = new AuthenticateResult
                {
                    Success = false,
                    ErrorMessages = new string[] { "User doesnt exist" }
                };
                return result;
            }

            var validPassword = await userManager.CheckPasswordAsync(user, password);

            return await GenerateAuthenticationResultForUser(user);
        }

        public async Task<AuthenticateResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null) return new AuthenticateResult
            {
                ErrorMessages = new[] { "Invalid token" }
            };

            var jti = validatedToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            var refreshTokenExists = await dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);

            var expiryDateUnix = long.Parse(validatedToken.Claims.Where(c => c.Type == JwtRegisteredClaimNames.Exp).SingleOrDefault().Value);

            var expirationDateUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).
                AddSeconds(expiryDateUnix);

            if (expirationDateUtc > DateTime.UtcNow)
            {
                return new AuthenticateResult
                {
                    ErrorMessages = new[] { "Invalid token" }
                };
            }

            if (refreshTokenExists == null)
            {
                return new AuthenticateResult
                {
                    ErrorMessages = new[] { "This token doesnt exist " }
                };
            }

            if (refreshTokenExists.Used)
            {
                return new AuthenticateResult
                {
                    ErrorMessages = new[] { "Token already used" }
                };
            }

            if (refreshTokenExists.Invalidated)
            {
                return new AuthenticateResult
                {
                    ErrorMessages = new[] { "Token is invalidated" }
                };
            }

            refreshTokenExists.Used = true;
            dataContext.RefreshTokens.Update(refreshTokenExists);
            await dataContext.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(validatedToken.Claims.SingleOrDefault(x => x.Type == "id").Value);

            return await GenerateAuthenticationResultForUser(user);
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

            try
            {
                if (!(validatedToken is JwtSecurityToken jwtSecurityToken &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)))
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
            return principal;
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
                Email = email,
            };

            var createdUser = await userManager.CreateAsync(user, password);

            if (!createdUser.Succeeded)
            {
                return new AuthenticateResult
                {
                    ErrorMessages = createdUser.Errors.Select(x => x.Description)
                };
            }

            return await GenerateAuthenticationResultForUser(user);
        }

        private async Task<AuthenticateResult> GenerateAuthenticationResultForUser(IdentityUser user)
        {
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
                Expires = DateTime.UtcNow.AddMilliseconds(jwtSettings.TokenLifetime.TotalSeconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken()
            {
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                UserId = user.Id,
                JwtId = token.Id
            };

            await dataContext.RefreshTokens.AddAsync(refreshToken);
            await dataContext.SaveChangesAsync();

            return new AuthenticateResult
            {
                Success = true,
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token.ToString()
            };

        }
    }
}
