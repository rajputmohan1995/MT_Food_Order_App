using Microsoft.EntityFrameworkCore;
using MT.Services.EmailAPI.DBContext;
using MT.Services.EmailAPI.Extensions;
using MT.Services.EmailAPI.Messaging;
using MT.Services.EmailAPI.Messaging.Interface;
using MT.Services.EmailAPI.Service;
using MT.Services.EmailAPI.Service.Interfaces;
using MT.Services.EmailAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<EmailDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var mapper = MapperConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

var optionBuilder = new DbContextOptionsBuilder<EmailDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddSingleton(serviceDescriptor => new EmailService(optionBuilder.Options, serviceDescriptor.GetRequiredService<IProductService>(),
    builder.Configuration, builder.Environment));


builder.Services.AddHttpClient("Product", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigrations();
app.UseAzureServiceBusConsumer();
app.Run();

void ApplyMigrations()
{
    using var scope = app.Services.CreateScope();
    var _db = scope.ServiceProvider.GetRequiredService<EmailDbContext>();
    if (_db.Database.GetPendingMigrations().Count() > 0)
        _db.Database.Migrate();
}