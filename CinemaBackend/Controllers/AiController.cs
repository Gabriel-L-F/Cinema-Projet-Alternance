using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CinemaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AiController : ControllerBase
    {
        private readonly MovieAiService _aiService;

        public AiController(MovieAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            // DIAGNOSTIC 1 : Est-ce que la requête arrive d'Angular ?
            Console.WriteLine("\n[DIAGNOSTIC] --> Une requête vient d'arriver depuis le Front-end !");
            
            if (request == null || string.IsNullOrEmpty(request.Message))
            {
                Console.WriteLine("[DIAGNOSTIC] ❌ Le message reçu est vide ou nul.");
                return BadRequest(new { reponse = "Le message ne peut pas être vide." });
            }

            Console.WriteLine($"[DIAGNOSTIC] 💬 Message de l'utilisateur : '{request.Message}'");

            try
            {
                // On passe la main au service
                var reponseIa = await _aiService.DemanderConseilAsync(request.Message);
                
                Console.WriteLine("[DIAGNOSTIC] ✅ Réponse reçue avec succès de Gemini !");
                return Ok(new { reponse = reponseIa });
            }
            catch (System.Exception ex)
            {
                // DIAGNOSTIC 2 : Si ça plante, on affiche TOUT le détail dans le terminal .NET
                Console.WriteLine($"[DIAGNOSTIC] ❌ ERREUR CAPTURÉE : {ex.Message}");
                
                // On renvoie l'erreur brute à Angular pour qu'elle s'affiche dans la bulle de chat
                return StatusCode(500, new { reponse = $"[Erreur Serveur] : {ex.Message}" });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
    }
}