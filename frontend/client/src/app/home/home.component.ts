import { Component } from '@angular/core';
import { SongComponent } from '../song/song.component';
import { GameComponent } from '../game/game.component';
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-home',
  imports: [SongComponent, GameComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export default class HomeComponent {

}
