namespace CinemaBackend.Models;

public class Film
{
    public int Id { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int DureeMinutes { get; set; }
    public int AgeMinimum { get; set; }
    public string? PosterPath { get; set; }
}