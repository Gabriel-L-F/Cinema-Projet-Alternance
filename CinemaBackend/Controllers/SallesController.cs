using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaBackend.Data;
using CinemaBackend.Models;

namespace CinemaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SallesController : ControllerBase
{
    private readonly CinemaDbContext _context;

    public SallesController(CinemaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Salle>>> GetSalles()
    {
        var salles = await _context.Salles.ToListAsync();
        return Ok(salles); 
    }

    [HttpPost]
    public async Task<ActionResult<Salle>> PostSalle(Salle nouvelleSalle)
    {
        _context.Salles.Add(nouvelleSalle);
        await _context.SaveChangesAsync();
        return Ok(nouvelleSalle); 
    }
}