using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using CinemaBackend.Data;   
using CinemaBackend.Models; 

public static class DataSeeder
{
    private static readonly Dictionary<int, string> TMDBGenresEnFrancais = new()
    {
        { 28, "Action" },
        { 12, "Aventure" },
        { 16, "Animation" },
        { 35, "Comédie" },
        { 80, "Policier" },
        { 99, "Documentaire" },
        { 18, "Drame" },
        { 10751, "Famille" },
        { 14, "Fantastique" },
        { 36, "Histoire" },
        { 27, "Horreur" },
        { 10402, "Musique" },
        { 9648, "Mystère" },
        { 10749, "Romance" },
        { 878, "Science-Fiction" },
        { 10770, "Téléfilm" },
        { 53, "Thriller" },
        { 10752, "Guerre" },
        { 37, "Western" }
    };

    public static async Task SeedMoviesAsync(CinemaDbContext context, IHttpClientFactory httpClientFactory, string? tmdbToken)
    {
        if (context.Films.Any()) return;

        if (string.IsNullOrEmpty(tmdbToken))
        {
            Console.WriteLine("--> Erreur : Le token TMDB est introuvable dans appsettings.Development.json");
            return;
        }

        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tmdbToken);
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

                    string? tmdbPosterPath = element.TryGetProperty("poster_path", out var posterProp) 
                        ? posterProp.GetString() 
                        : null;

                    string nomDuGenre = "Cinéma"; 
                    if (element.TryGetProperty("genre_ids", out var genreIdsProp) && genreIdsProp.GetArrayLength() > 0)
                    {
                        int premierGenreId = genreIdsProp.EnumerateArray().First().GetInt32();
                        
                        if (TMDBGenresEnFrancais.TryGetValue(premierGenreId, out var genreTraduit))
                        {
                            nomDuGenre = genreTraduit;
                        }
                    }

                    var film = new Film
                    {
                        Titre = element.GetProperty("title").GetString() ?? "Sans titre",
                        Description = element.GetProperty("overview").GetString() ?? "Pas de description.",
                        Genre = nomDuGenre, 
                        DureeMinutes = randomDuree,
                        AgeMinimum = randomAge,
                        PosterPath = tmdbPosterPath
                    };

                    context.Films.Add(film);
                }

                await context.SaveChangesAsync();
                Console.WriteLine("--> BDD peuplée avec succès avec les vrais genres et affiches !");
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