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

namespace TweetAPI.Installer
{
    public class MvcInstaller : IInstaller
    {
        public void InstallSevices(IConfiguration configuration, IServiceCollection services)
        {
            var jwtSettings = new JWTSettings();
            configuration.Bind(nameof(JWTSettings), jwtSettings);

            services.AddSingleton<JWTSettings>();

            services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true
                };

            });



            services.AddControllersWithViews();
            services.AddSwaggerGen(action =>
            {
                action.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "TweetAPI", Version = "v1" });
                var securityToken = new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[0] }
                };

                action.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Scheme = "ApiKeyScheme",
                    Name = "Authorization",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
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
