using System.Security.Claims;

namespace Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateAccesToken(IEnumerable<Claim> claims);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalFromToken(string token);
    }
}
