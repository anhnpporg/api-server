using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace UtNhanDrug_BE.Configurations
{
    public static class SecurityConfig
    {
        public static IServiceCollection RegisterSecurityModule(this IServiceCollection services, IConfiguration configuration)
        {
            // Load Setting From appsetting.json
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var twilioSettingsSection = configuration.GetSection("TwilioConfig");
            services.Configure<TwilioConfig>(twilioSettingsSection);

            // Map Setting to class AppSetting
            var appSettings = twilioSettingsSection.Get<TwilioConfig>();
            var key = Encoding.ASCII.GetBytes(appSettings.AuthToken);

            // Set Authentication to verify token
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }

        public static IApplicationBuilder UseApplicationSecurity(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
