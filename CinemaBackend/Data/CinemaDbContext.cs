using Microsoft.EntityFrameworkCore;
using CinemaBackend.Models;

namespace CinemaBackend.Data;

public class CinemaDbContext : DbContext
{
    public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
    {
    }

    public DbSet<Film> Films { get; set; } = null!;
    public DbSet<Salle> Salles { get; set; } = null!;
    public DbSet<Seance> Seances { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
}