using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetAPI.Controllers.V1;
using TweetAPI.Services;

namespace TweetAPI.Installer
{
    public class MvcInstaller : IInstaller
    {
        public void InstallSevices(IConfiguration configuration, IServiceCollection services)
        {
            var jwtSettings = new JWTSettings();
            configuration.GetSection("JWTSettings").Bind(jwtSettings);

            services.AddScoped<IIdentityService, IdentityService>();
            services.AddSingleton(jwtSettings);

            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddControllersWithViews();
            services.AddSwaggerGen(action =>
            {
                action.SwaggerDoc("v1", new OpenApiInfo { Title = "TweetAPI", Version = "v1" });
                var securityToken = new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[0] }
                };

                action.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "ApiKeyScheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                var openApiSecurityRequirement = new OpenApiSecurityRequirement {
                    {   new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                    } }, new string[] { }
                    }
                };
                action.AddSecurityRequirement(openApiSecurityRequirement);
            });
            services.AddRazorPages();
        }
    }
}
