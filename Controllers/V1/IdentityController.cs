using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetAPI.Contracts.V1;
using TweetAPI.Controllers.V1.Requests;

namespace TweetAPI.Controllers.V1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var registrationResult = await _identityService.RegisterUserAsync(request.Email, request.Password);
            if (!registrationResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = registrationResult.ErrorMessages
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = registrationResult.Token,
                RefreshToken = registrationResult.RefreshToken.ToString()
            });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(o => o.Errors.Select(x => x.ErrorMessage))
                });
            }

            var loginResult = await _identityService.LoginAsync(request.Email, request.Password);
            if (!loginResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = loginResult.ErrorMessages
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = loginResult.Token,
                RefreshToken = loginResult.RefreshToken.ToString()
            });
        }

        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(o => o.Errors.Select(x => x.ErrorMessage))
                });
            }

            var loginResult = await _identityService.RefreshTokenAsync(request.Token, request.RefreshToken);
            if (!loginResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = loginResult.ErrorMessages
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = loginResult.Token,
                RefreshToken = loginResult.RefreshToken.ToString()
            });
        }
    }
}
