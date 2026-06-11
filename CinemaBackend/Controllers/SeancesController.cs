using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaBackend.Data;
using CinemaBackend.Models;

namespace CinemaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeancesController : ControllerBase
{
    private readonly CinemaDbContext _context;

    public SeancesController(CinemaDbContext context)
    {
        _context = context;
    }

    // 1. Récupérer toutes les séances avec les détails du Film et de la Salle
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Seance>>> GetSeances()
    {
        var seances = await _context.Seances
            .Include(s => s.Film)
            .Include(s => s.Salle)
            .ToListAsync();

        return Ok(seances);
    }

    // 2. Ajouter une nouvelle séance
    [HttpPost]
    public async Task<ActionResult<Seance>> PostSeance(Seance nouvelleSeance)
    {
        _context.Seances.Add(nouvelleSeance);
        await _context.SaveChangesAsync();
        return Ok(nouvelleSeance);
    }
}