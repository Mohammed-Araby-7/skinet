using API.Middleware;
using API.SignalR;
using Core;
using Core.Enitites;
using Core.interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter()
        );
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

//connection string
builder.Services.AddDbContext<StoreContext>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("MyConn"));
});
builder.Services.AddScoped<IProductRepo,ProductRepo>();
builder.Services.AddScoped(typeof(IGenericRepo<>),typeof(GenericRepo<>));   
builder.Services.AddScoped(typeof(IUnitOfWork),typeof(UnitOfWork));   
builder.Services.AddCors();
builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connString = builder.Configuration.GetConnectionString("Redis")
    ?? throw new Exception("Cannot get redis connection string");
    var configuration = ConfigurationOptions.Parse(connString,true);
    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICartService,CartService>();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
        .AddEntityFrameworkStores<StoreContext>();
builder.Services.AddScoped<IPaymentService,PaymentService>();
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x=> x.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200","https://localhost:4200"));
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Serve Angular static files
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>();
app.MapHub<NotificationHub>("/hub/notifications");
app.MapFallbackToController("Index" , "Fallback");
// Angular routing fallback
// app.MapFallbackToFile("index.html");
try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch(System.Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
