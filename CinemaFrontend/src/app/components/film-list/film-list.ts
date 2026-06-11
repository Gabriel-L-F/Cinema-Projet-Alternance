import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FilmService, Film } from '../../services/film';

@Component({
  selector: 'app-film-list',
  standalone: true,
  imports: [CommonModule], 
  templateUrl: './film-list.html',
  styleUrl: './film-list.scss'
})

export class FilmListComponent implements OnInit {
  films: Film[] = [];

  constructor(private filmService: FilmService) {}

  ngOnInit(): void {
    this.chargerFilms();
  }

  chargerFilms(): void {
    this.filmService.getFilms().subscribe({
      next: (donnees) => {
        this.films = donnees; 
      },
      error: (erreur) => {
        console.error('Erreur lors de la récupération des films :', erreur);
      }
    });
  }

ajouterFilmTest(): void {
    const nouveauFilm: Film = {
      titre: 'Inception',
      description: 'Un voleur spécialisé dans l\'extraction de secrets enfouis dans le subconscient pendant le rêve est chargé d\'implanter une idée dans l\'esprit d\'un ciblé.',
      genre: 'Science-Fiction',
      dureeMinutes: 148,
      ageMinimum: 12
    };

    this.filmService.addFilm(nouveauFilm).subscribe({
      next: (filmCree) => {
        console.log('Film créé avec succès !', filmCree);
        this.chargerFilms();
      },
      error: (erreur) => {
        console.error('Erreur lors de la création du film :', erreur);
      }
    });
  }
}