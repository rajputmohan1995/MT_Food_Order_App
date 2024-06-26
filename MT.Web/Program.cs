using Microsoft.AspNetCore.Authentication.Cookies;
using MT.Web.Service;
using MT.Web.Service.Interface;
using MT.Web.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"] ?? "";
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"] ?? "";
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"] ?? "";
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"] ?? "";
SD.OrderAPIBase = builder.Configuration["ServiceUrls:OrderAPI"] ?? "";

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddHttpClient<IProductService, ProductService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<ICartService, CartService>();
builder.Services.AddHttpClient<IOrderService, OrderService>();

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.ExpireTimeSpan = TimeSpan.FromHours(10);
                    options.LoginPath = "/auth/login";
                    options.AccessDeniedPath = "/auth/login";
                });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
