import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FilmListComponent } from './components/film-list/film-list';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FilmListComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('CinemaFrontend');
}
