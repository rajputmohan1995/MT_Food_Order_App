using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MT.Services.CouponAPI.DBContext;
using Microsoft.OpenApi.Models;
using MT.Services.CouponAPI.Extensions;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<CouponDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var mapper = MapperConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the bearer authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id= JwtBearerDefaults.AuthenticationScheme
                }
            },
            new string[]{ }
        }
    });
});

StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("StripeSecretKey");

builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{ 
//}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Coupon API");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

ApplyMigrations();

app.Run();


void ApplyMigrations()
{
    using var scope = app.Services.CreateScope();
    var _db = scope.ServiceProvider.GetRequiredService<CouponDbContext>();
    if (_db.Database.GetPendingMigrations().Count() > 0)
        _db.Database.Migrate();
}