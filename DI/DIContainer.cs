using Data.Context;
using Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Jwt;

namespace EventApp.DI
{
    public static class DIContainer
    {
        public static void RegisterDependency(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowWithCredentials", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });

            services.AddDbContext<EventAppDbContext>(options =>
                options.UseSqlServer(configuration.GetSection("").Value));

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<EventAppDbContext>()
                .AddSignInManager<SignInManager<User>>()
                .AddUserManager<UserManager<User>>()
                .AddRoles<IdentityRole>();

            services.AddAuthorizationBuilder()
                .AddPolicy("AdminPolicy", options => {
                    options.RequireAuthenticatedUser();
                    options.RequireRole("admin");
                });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                IServiceProvider serviceProvider = services.BuildServiceProvider();

                JwtOptions jwtOptions = serviceProvider.GetService<IOptions<JwtOptions>>()!.Value;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience =false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtOptions.GetSymmetricSecurityKey()
                };
            });
        }
    }
}
