using Application.Interface;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISpotifyLogin _spotifyService;
        public AuthController(ISpotifyLogin spotifyService)
        {
            _spotifyService = spotifyService;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Authcode([FromQuery] string code, [FromQuery] string state)
        {
            await _spotifyService.ExchangeCodeForAccessToken(code, state);
            return Redirect("/");
        }
    }
}
