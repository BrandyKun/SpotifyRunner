using Application.Interface;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISpotifyLogin _spotifyService;
        private readonly IOptions<AppSettings> _appSettings;
        public AuthController(ISpotifyLogin spotifyService, IOptions<AppSettings> appsettings)
        {
            _appSettings = appsettings;
            _spotifyService = spotifyService;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Authcode([FromQuery] string code, [FromQuery] string state)
        {
            await _spotifyService.ExchangeCodeForAccessToken(code, state, _appSettings.Value.ClientId, _appSettings.Value.ClientSecret);
            return Redirect("/");
        }
    }
}
