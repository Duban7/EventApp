using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Jwt
{
    public class JwtOptions
    {
        public string? Key { get; set; }
        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new(System.Text.Encoding.UTF8.GetBytes(Key!));
    }
}
