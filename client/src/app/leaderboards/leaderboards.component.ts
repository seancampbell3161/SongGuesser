import { Component, effect, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { GameService } from '../game/data-access/game.service';
import { NavComponent } from '../nav/nav.component';
import { TableModule } from 'primeng/table';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-leaderboards',
  imports: [NavComponent, TableModule, NgClass],
  templateUrl: './leaderboards.component.html',
  styleUrl: './leaderboards.component.css'
})
export default class LeaderboardsComponent {
  private gameSvc = inject(GameService);

  isDarkMode = signal<boolean | undefined>(undefined);
  leaderboard = toSignal(this.gameSvc.loadLeaderboard$);

  toggleDarkmode(isDarkMode: boolean) {
    this.isDarkMode.set(isDarkMode);
  }

}
