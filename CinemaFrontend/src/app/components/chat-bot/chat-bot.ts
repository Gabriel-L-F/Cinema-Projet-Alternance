import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AiService } from '../../services/ai';
interface Message {
  texte: string;
  expediteur: 'user' | 'ia';
  date: Date;
}

@Component({
  selector: 'app-chat-bot',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat-bot.html',
  styleUrls: ['./chat-bot.scss']
})
export class ChatBotComponent {
  messageSaisi: string = '';
  historiqueMessages: Message[] = [];
  estEnTrainDeCharger: boolean = false;
  estOuvert: boolean = false;

  constructor(private aiService: AiService) {
    this.historiqueMessages.push({
      texte: "Bonjour ! Je suis le conseiller cinéma de Gabriel. Quel type de film recherchez-vous aujourd'hui ?",
      expediteur: 'ia',
      date: new Date()
    });
  }

  basculerChat() {
    this.estOuvert = !this.estOuvert;
  }

  envoyer() {
    if (!this.messageSaisi.trim() || this.estEnTrainDeCharger) return;

    const messageUtilisateur = this.messageSaisi;
    this.historiqueMessages.push({
      texte: messageUtilisateur,
      expediteur: 'user',
      date: new Date()
    });

    this.messageSaisi = '';
    this.estEnTrainDeCharger = true;

    this.aiService.envoyerMessage(messageUtilisateur).subscribe({
      next: (response) => {
        this.historiqueMessages.push({
          texte: response.reponse,
          expediteur: 'ia',
          date: new Date()
        });
        this.estEnTrainDeCharger = false;
      },
      error: (err) => {
        console.error("Erreur Backend / Gemini :", err);
        this.historiqueMessages.push({
          texte: "Mince, le serveur .NET ne répond pas. Vérifie qu'il est bien lancé !",
          expediteur: 'ia',
          date: new Date()
        });
        this.estEnTrainDeCharger = false;
      }
    });
  }
}