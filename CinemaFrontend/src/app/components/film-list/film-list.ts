import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; 
import { FilmService, Film } from '../../services/film';

@Component({
  selector: 'app-film-list',
  standalone: true,
  imports: [CommonModule, FormsModule], 
  templateUrl: './film-list.html',
  styleUrl: './film-list.scss'
})
export class FilmListComponent implements OnInit {
  films: any[] = [];

  nouveauFilm: Film = {
    titre: '',
    description: '',
    genre: '',
    dureeMinutes: 120,
    ageMinimum: 0,   
    posterPath: ''
  };

  constructor(
    private filmService: FilmService,
    private cdr: ChangeDetectorRef 
  ) {}

  ngOnInit(): void {
    this.chargerFilms();
  }

  chargerFilms(): void {
    this.filmService.getFilms().subscribe({
      next: (donnees: any[]) => {
        this.films = donnees.map(film => ({
          ...film,
          afficheUrl: film.posterPath || film.poster_path || null
        }));
        this.cdr.detectChanges(); 
      },
      error: (erreur) => {
        console.error('Erreur lors de la récupération des films :', erreur);
      }
    });
  }

  ajouterNouveauFilm(): void {
    if (!this.nouveauFilm.titre.trim()) return;

    this.filmService.addFilm(this.nouveauFilm).subscribe({
      next: (filmCree) => {
        console.log('Film ajouté avec succès !', filmCree);
        
        this.chargerFilms(); 
        
        this.nouveauFilm = {
          titre: '',
          description: '',
          genre: '',
          dureeMinutes: 120,
          ageMinimum: 0,
          posterPath: ''
        };
      },
      error: (erreur) => {
        console.error('Erreur lors de l\'ajout du film :', erreur);
      }
    });
  }
}