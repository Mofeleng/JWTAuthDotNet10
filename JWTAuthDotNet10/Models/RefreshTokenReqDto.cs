namespace JWTAuthDotNet10.Models
{
    public class RefreshTokenReqDto
    {
        public Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
