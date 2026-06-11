namespace CinemaBackend.Models;

public class Reservation
{
    public int Id { get; set; }
    public string NomClient { get; set; } = string.Empty;
    public int NombrePlaces { get; set; }
    
    public int SeanceId { get; set; }
    public Seance Seance { get; set; } = null!;
}