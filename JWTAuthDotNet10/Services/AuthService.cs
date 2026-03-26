using JWTAuthDotNet10.Data;
using JWTAuthDotNet10.Entities;
using JWTAuthDotNet10.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JWTAuthDotNet10.Services
{
    public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<TokenResponseDto?> LoginAysnc(UserDto req)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user is null) return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, req.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await GenerateTokenResponse(user);
        }

        private async Task<TokenResponseDto> GenerateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = GenerateJWTToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<User?> RegisterAysnc(UserDto req)
        {
            if (await context.Users.AnyAsync(u => u.Username == req.Username))
            {
                return null;
            }
            var user = new User();

            var hashedPassword = new PasswordHasher<User>()
                                    .HashPassword(user, req.Password);

            user.Username = req.Username;
            user.PasswordHash = hashedPassword;

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenReqDto req)
        {
            var user = await ValidateRefreshTokenAsync(req.UserId, req.RefreshToken);
            if (user is null) return null;

            return await GenerateTokenResponse(user);

        }

        private string GenerateRefreshToken() {
            var randNumber = new byte[32];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randNumber);

            return Convert.ToBase64String(randNumber);
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await context.Users.FindAsync(userId);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTOkenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }



        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTOkenExpiryTime = DateTime.UtcNow.AddDays(7);
            await context.SaveChangesAsync();

            return refreshToken;
        }

        private string GenerateJWTToken(User user)
        {
            //Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };


            //Security key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!)
            );

            //Signing credentials
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512); //Token 64 chars
            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }
    }
}
