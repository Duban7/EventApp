using Microsoft.IdentityModel.Tokens;

namespace Services.Jwt
{
    public class JwtOptions
    {
        public string? Key { get; set; }
        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new(System.Text.Encoding.UTF8.GetBytes(Key!));
    }
}
