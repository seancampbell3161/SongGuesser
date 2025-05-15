import { Component, signal } from '@angular/core';
import { SongComponent } from '../song/song.component';
import { GameComponent } from '../game/game.component';
import { NgxSpinnerComponent } from 'ngx-spinner';
import { ButtonModule } from 'primeng/button';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [SongComponent, GameComponent, ButtonModule, NgClass],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export default class HomeComponent {
  isDarkMode = signal(false);

  toggleDarkMode() {
    const element = document.querySelector('html');
    element?.classList.toggle('darkmode');

    this.isDarkMode.set(element?.classList.contains('darkmode') ?? false);
  }
}
