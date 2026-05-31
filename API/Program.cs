using API.Middleware;
using Core.interfaces;
using Infrastructure;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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
builder.Services.AddCors();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x=> x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200","https://localhost:4200"));
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();

app.MapControllers();

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
