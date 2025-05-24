import { Component, inject } from '@angular/core';
import { SongService } from '../song/data-access/song.service';
import { ButtonModule } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { GameService } from './data-access/game.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { tap } from 'rxjs';

@Component({
  selector: 'app-game',
  imports: [ButtonModule, InputText, FormsModule],
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent {
  songSvc = inject(SongService);
  gameSvc = inject(GameService);

  guessResponse = toSignal(this.gameSvc.isGuessCorrect$.pipe(tap((isCorrect) => {
    if (!isCorrect) {
      this.songSvc.skipGuess$.next();
      this.guess = '';
    }
  })))

  guess: string = '';

  constructor() {

  }
}
