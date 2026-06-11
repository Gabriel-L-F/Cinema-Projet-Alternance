namespace CinemaBackend.Models;

public class Seance
{
    public int Id { get; set; }
    public DateTime HoraireDebut { get; set; }

    public int FilmId { get; set; }
    public int SalleId { get; set; }

    public Film Film { get; set; } = null!;
    public Salle Salle { get; set; } = null!;
}