using JWTAuthDotNet10.Entities;
using JWTAuthDotNet10.Models;

namespace JWTAuthDotNet10.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAysnc(UserDto req);
        Task<TokenResponseDto?> LoginAysnc(UserDto req);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenReqDto req);
    }
}
