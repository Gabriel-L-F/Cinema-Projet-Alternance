using Microsoft.EntityFrameworkCore;
using CinemaBackend.Data; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<CinemaBackend.Data.CinemaDbContext>(options =>
    options.UseInMemoryDatabase("CinemaDb"));

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CinemaDbContext>();
    var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
    
    await DataSeeder.SeedMoviesAsync(context, httpClientFactory);
}

app.Run(); 
