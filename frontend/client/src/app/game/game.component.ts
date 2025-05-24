import { Component, effect, inject } from '@angular/core';
import { SongService } from '../song/data-access/song.service';
import { ButtonModule } from 'primeng/button';
import { InputText } from 'primeng/inputtext';
import { FormsModule } from '@angular/forms';
import { GameService } from './data-access/game.service';

@Component({
  selector: 'app-game',
  imports: [ButtonModule, InputText, FormsModule],
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent {
  songSvc = inject(SongService);
  gameSvc = inject(GameService);

  guess: string = '';

  constructor() {

  }
}
