using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using CinemaBackend.Data;   
using CinemaBackend.Models; 

public static class DataSeeder
{
    private const string TmdbToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiI4NjExNDdhY2E2YWM2YjVhMGZiNGM2MTRkMGZjMWMxYSIsIm5iZiI6MTc4MTE3MjU5OC4xMTM5OTk4LCJzdWIiOiI2YTJhODk3NmEyMDZlY2MxYjhmYzM0YzgiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.U8VLc6uCqvZb9lRM6uq9aD_IIsh_57WfXqgs0dvNLDY";

    public static async Task SeedMoviesAsync(CinemaDbContext context, IHttpClientFactory httpClientFactory)
    {
        if (context.Films.Any()) return;

        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TmdbToken);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            var response = await client.GetAsync("https://api.themoviedb.org/3/movie/popular?language=fr-FR&page=1");
            
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;
                var results = root.GetProperty("results");

                foreach (var element in results.EnumerateArray())
                {
                    var random = new Random();
                    int randomDuree = random.Next(90, 160); 
                    int randomAge = random.Next(0, 3) switch { 0 => 0, 1 => 12, _ => 16 };

                    var film = new Film
                    {
                        Titre = element.GetProperty("title").GetString() ?? "Sans titre",
                        Description = element.GetProperty("overview").GetString() ?? "Pas de description.",
                        Genre = "Cinéma", 
                        DureeMinutes = randomDuree,
                        AgeMinimum = randomAge
                    };

                    context.Films.Add(film);
                }

                await context.SaveChangesAsync();
                Console.WriteLine("--> BDD peuplée avec succès grâce à TMDB !");
            }
            else
            {
                Console.WriteLine($"--> TMDB a répondu avec une erreur : {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Erreur lors du peuplement TMDB : {ex.Message}");
        }
    }
}