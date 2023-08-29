﻿using Application.Interface;
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

        /// <summary>
        /// Using the authcode from the url
        /// is used to retireve bearer token
        /// for oAuth auth code flow.
        /// </summary>
        /// <param name="code">string from query string</param>
        /// <param name="state">string from state</param>
        /// <returns>Token</returns>
        [HttpGet("callback")]
        public async Task<AuthResult> Authcode([FromQuery] string code, [FromQuery] string state)
        {
            AuthResult token = new AuthResult();
            if (code != null && state != null)
            {
                token = await _spotifyService.ExchangeCodeForAccessToken(code, state);

            }
            if (token == null)
                return null;

            return token;
        }

        [HttpPost, Route("Token")]
        public async Task<string> RequestToken()
        {
            var token = await _spotifyService.GetToken( "something");
            var gotThings = token;
            return "true";
        }

        /// <summary>
        /// Method to log in on spotify 
        /// using AuthCode oAuth Flow
        /// </summary>
        /// <returns>redirect url in a string</returns>
        [HttpGet, Route("authCode")]
        public Task<string> RequestAuthCode()
        {
            return Task.Run(() =>
            {
                var address = _spotifyService.BuildUri(
                    
                );
                return address.ToString();
            });
        }
    }
}
