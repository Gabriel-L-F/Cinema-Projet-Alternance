using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CinemaBackend.Data;
using Microsoft.Extensions.Configuration;

public class MovieAiService
{
    private readonly CinemaDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _mistralApiKey;

    public MovieAiService(CinemaDbContext dbContext, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
        
        // Récupération de la clé Mistral depuis appsettings.Development.json
        _mistralApiKey = configuration["ApiSettings:MistralApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "La clé API Mistral est manquante dans la configuration.");
    }

    public async Task<string> DemanderConseilAsync(string messageUtilisateur)
    {
        // 1. Récupération des films de ta base de données
        var filmsEnBase = _dbContext.Films.ToList();
        var filmsJson = JsonSerializer.Serialize(filmsEnBase);

        // 2. Configuration des instructions (System Prompt)
        string systemPrompt = "Tu es un conseiller de cinéma virtuel expert. Tu travailles pour l'application de Gabriel. " +
                              "Tu dois RECOMMANDER UNIQUEMENT des films présents dans la liste ci-dessous. " +
                              "Si aucun film de la liste ne correspond à la demande, explique-le poliment et propose l'alternative la plus proche. " +
                              "Sois chaleureux, concis et réponds en français.\n\n" +
                              $"LISTE DES FILMS DISPONIBLES EN BDD :\n{filmsJson}";

        // 3. Endpoint officiel Chat Completions de Mistral AI
        string url = "https://api.mistral.ai/v1/chat/completions";

        // 4. Construction du Payload au format ChatML attendu par Mistral
        var payload = new
        {
            model = "mistral-small-latest", // Modèle idéal pour le dev : rapide et intelligent
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = messageUtilisateur }
            },
            temperature = 0.7
        };

        // 5. Préparation de la requête HTTP
        var client = _httpClientFactory.CreateClient();
        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // 6. Authentification standard Bearer (Injecte ta clé iALVG...)
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _mistralApiKey);

        // 7. Envoi de la requête à Mistral
        var response = await client.PostAsync(url, content);
        string responseString = await response.Content.ReadAsStringAsync();

        // 8. Vérification des erreurs
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erreur Mistral API ({response.StatusCode}) : {responseString}");
        }

        // 9. Parsing du JSON pour extraire uniquement le texte de la réponse
        using var doc = JsonDocument.Parse(responseString);
        var textReponse = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return textReponse ?? "Désolé, je n'ai pas pu générer de recommandation.";
    }
}