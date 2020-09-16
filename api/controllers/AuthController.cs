using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace SS.Api.Controllers
{
    public class AuthController : ControllerBase
    {
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        [HttpGet("login")]
        public IActionResult Login(string redirectUri)
        {
            redirectUri = redirectUri ?? "https://localhost:44369/api";
            return Redirect(redirectUri);
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet("get_token")]
        public async Task<IActionResult> GetToken()
        {
            var accessToken = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            var refreshToken = await HttpContext.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "refresh_token");
            return Ok(new { access_token = accessToken, refresh_token = refreshToken });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string redirectUri)
        {
            redirectUri = redirectUri ?? "https://localhost:44369/api";
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);
            return Redirect(redirectUri);
        }
    }
}
    