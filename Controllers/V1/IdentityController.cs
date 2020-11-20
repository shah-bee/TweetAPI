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
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request) {

            var registrationResult = await _identityService.RegisterUserAsync(request.Email, request.Password);

            if (!registrationResult.Success) {
                return BadRequest(new AuthenticationResult { 
                   Messages = registrationResult.ErrorMessages
                });
                    
            }

            return Ok(new AuthenticationResult { Token = registrationResult.Token });
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid) {
                return BadRequest(new AuthenticationResult
                {
                    Messages = ModelState.Values.SelectMany(o => o.Errors.Select(x => x.ErrorMessage))
                });
            }

            var loginResult = await _identityService.LoginUserAsync(request.Email, request.Password);

            if (!loginResult.Success)
            {
                return BadRequest(new AuthenticationResult
                {
                    Messages = loginResult.ErrorMessages
                });

            }

            return Ok(new AuthenticationResult { Token = loginResult.Token });
        }
    }
}
