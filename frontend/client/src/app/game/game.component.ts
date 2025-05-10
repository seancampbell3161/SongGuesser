import { Component, inject } from '@angular/core';
import { SongService } from '../song/data-access/song.service';

@Component({
  selector: 'app-game',
  imports: [],
  templateUrl: './game.component.html',
  styleUrl: './game.component.css'
})
export class GameComponent {
  songSvc = inject(SongService);
}
