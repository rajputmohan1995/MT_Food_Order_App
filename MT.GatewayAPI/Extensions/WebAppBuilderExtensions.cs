using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MT.GatewayAPI.Extensions;

public static class WebAppBuilderExtensions
{
    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {
        var secret = builder.Configuration.GetValue<string>("ApiSettings:Secret") ?? string.Empty;
        var issuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer") ?? string.Empty;
        var audience = builder.Configuration.GetValue<string>("ApiSettings:Audience") ?? string.Empty;

        var key = Encoding.ASCII.GetBytes(secret);

        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
            };
        });

        return builder;
    }
}