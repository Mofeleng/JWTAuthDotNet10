using JWTAuthDotNet10.Models;
using JWTAuthDotNet10.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using JWTAuthDotNet10.Services;
using Microsoft.AspNetCore.Authorization;

namespace JWTAuthDotNet10.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto req)
        {
            var user = await authService.RegisterAysnc(req);
            if (user is null)
            {
                return BadRequest("Username already exists");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(UserDto req)
        {
            var res = await authService.LoginAysnc(req);
            if (res is null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(res);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenReqDto req)
        {
            var res = await authService.RefreshTokenAsync(req);
            if (res is null || res.AccessToken is null || res.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }
            return Ok(res);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthOnlyEndpoint()
        {
            return Ok("Welcome mortal!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("Welcome immortal creator!");
        }
    }
}
