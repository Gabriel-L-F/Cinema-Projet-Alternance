using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaBackend.Data;
using CinemaBackend.Models;

namespace CinemaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmsController : ControllerBase
{
    private readonly CinemaDbContext _context;

    public FilmsController(CinemaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Film>>> GetFilms()
    {
        var films = await _context.Films.ToListAsync();
        return Ok(films);
    }

    [HttpPost]
    public async Task<ActionResult<Film>> PostFilm(Film nouveauFilm)
    {
        _context.Films.Add(nouveauFilm);
        await _context.SaveChangesAsync();
        return Ok(nouveauFilm);
    }
}