import { Component, inject, signal } from '@angular/core';
import { SongComponent } from '../song/song.component';
import { GameComponent } from '../game/game.component';
import { ButtonModule } from 'primeng/button';
import { NavComponent } from '../nav/nav.component';
import { GameService } from '../game/data-access/game.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, tap } from 'rxjs';
import JSConfetti from 'js-confetti'
import { DialogModule } from 'primeng/dialog';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-home',
  imports: [SongComponent, GameComponent, ButtonModule, NavComponent, DialogModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export default class HomeComponent {
  private gameSvc = inject(GameService);
  private messageSvc = inject(MessageService);

  isDarkMode = signal<boolean | undefined>(undefined);
  showGameWon = signal(false);
  gameWon = toSignal(this.gameSvc.gameWon$.pipe(
    tap(() => {
      const jsConfetti = new JSConfetti();

      jsConfetti.addConfetti();
      this.showGameWon.set(true);
    })
  ));
  incorrectGuess = toSignal(this.gameSvc.isGuessCorrect$.pipe(
    filter(g => g === false),
    tap(() => this.messageSvc.add({ severity: 'warn', summary: 'Incorrect', detail: 'Incorrect! Try again' }))
  ));

  toggleDarkmode(isDarkMode: boolean) {
    this.isDarkMode.set(isDarkMode);
  }
}
