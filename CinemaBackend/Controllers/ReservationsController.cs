using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaBackend.Data;
using CinemaBackend.Models;

namespace CinemaBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly CinemaDbContext _context;

    public ReservationsController(CinemaDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Seance)
            .ThenInclude(s => s.Film)
            .ToListAsync();

        return Ok(reservations);
    }

    [HttpPost]
    public async Task<ActionResult<Reservation>> PostReservation(Reservation nouvelleReservation)
    {
        var seance = await _context.Seances
            .Include(s => s.Salle)
            .FirstOrDefaultAsync(s => s.Id == nouvelleReservation.SeanceId);

        if (seance == null)
        {
            return NotFound("Séance introuvable.");
        }

        var placesReservees = await _context.Reservations
            .Where(r => r.SeanceId == nouvelleReservation.SeanceId)
            .SumAsync(r => r.NombrePlaces);

        var placesDisponibles = seance.Salle.Capacite - placesReservees;

        if (nouvelleReservation.NombrePlaces > placesDisponibles)
        {
            return BadRequest($"Désolé, il ne reste que {placesDisponibles} places disponibles pour cette séance.");
        }

        _context.Reservations.Add(nouvelleReservation);
        await _context.SaveChangesAsync();

        return Ok(nouvelleReservation);
    }
}