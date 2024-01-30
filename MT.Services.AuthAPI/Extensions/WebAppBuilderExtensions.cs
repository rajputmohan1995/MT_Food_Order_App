using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MT.Services.AuthAPI.Extensions;

public static class WebAppBuilderExtensions
{
    public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
    {

        var secret = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Secret") ?? null;
        var issuer = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Issuer");
        var audience = builder.Configuration.GetValue<string>("ApiSettings:JwtOptions:Audience");

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