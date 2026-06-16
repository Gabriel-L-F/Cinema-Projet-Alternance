using Microsoft.EntityFrameworkCore;
using CinemaBackend.Data; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddScoped<MovieAiService>();

builder.Services.AddDbContext<CinemaDbContext>(options =>
    options.UseInMemoryDatabase("CinemaDb"));

builder.Services.AddOpenApi();


var app = builder.Build();

app.UseCors("AllowAngular");

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
    var configuration = services.GetRequiredService<IConfiguration>();
    
    var tmdbToken = configuration["ApiSettings:TmdbToken"];
    
    await DataSeeder.SeedMoviesAsync(context, httpClientFactory, tmdbToken);
}

app.Run();