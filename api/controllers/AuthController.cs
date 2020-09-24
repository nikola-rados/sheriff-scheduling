using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SS.Api.services;

namespace SS.Api.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// This cannot be called from AJAX or SWAGGER. It must be loaded in the browser location, because it brings the user to the SSO page. 
        /// </summary>
        /// <param name="redirectUri">URL to go back to.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        [HttpGet("login")]
        public async Task<IActionResult> Login(string redirectUri = "")
        {
            //Create / update users from claims. 
            var user = await _authService.UpsertUserFromClaims((ClaimsIdentity)User.Identity);
            if (user.IsDisabled)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }
            return Ok();
            //return Redirect(redirectUri);
        }

        /// <summary>
        /// Must be logged in to call this. 
        /// </summary>
        /// <returns>access_token and refresh_token for API calls.</returns>
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("get_token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetToken()
        {
            var accessToken = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            var refreshToken = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "refresh_token");
            return Ok(new { access_token = accessToken, refresh_token = refreshToken });
        }

        /// <summary>
        /// Logout function, should wipe out all cookies. 
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}
    