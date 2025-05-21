import { Component, signal } from '@angular/core';
import { SongComponent } from '../song/song.component';
import { GameComponent } from '../game/game.component';
import { ButtonModule } from 'primeng/button';
import { NavComponent } from '../nav/nav.component';

@Component({
  selector: 'app-home',
  imports: [SongComponent, GameComponent, ButtonModule, NavComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export default class HomeComponent {
  isDarkMode = signal<boolean | undefined>(undefined);

  toggleDarkmode(isDarkMode: boolean) {
    this.isDarkMode.set(isDarkMode);
  }
}
