import { Component, inject, signal } from '@angular/core';
import { SongComponent } from '../song/song.component';
import { GameComponent } from '../game/game.component';
import { ButtonModule } from 'primeng/button';
import { NavComponent } from '../nav/nav.component';
import { GameService } from '../game/data-access/game.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { tap } from 'rxjs';
import JSConfetti from 'js-confetti'
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-home',
  imports: [SongComponent, GameComponent, ButtonModule, NavComponent, NgClass],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export default class HomeComponent {
  private gameSvc = inject(GameService);

  isDarkMode = signal<boolean | undefined>(undefined);
  gameWon = toSignal(this.gameSvc.gameWon$.pipe(
    tap(() => {
      const jsConfetti = new JSConfetti()

      jsConfetti.addConfetti()
    })
  ));

  toggleDarkmode(isDarkMode: boolean) {
    this.isDarkMode.set(isDarkMode);
  }
}
