import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FilmListComponent } from './components/film-list/film-list';
import { ChatBotComponent } from './components/chat-bot/chat-bot';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, FilmListComponent, ChatBotComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('CinemaFrontend');
}
