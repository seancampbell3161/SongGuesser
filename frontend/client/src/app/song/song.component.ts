import { Component, computed, effect, inject, signal } from '@angular/core';
import { SongService } from './data-access/song.service';
import { Howl } from 'howler';
import { NgClass } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { tap } from 'rxjs';
import { BusyService } from '../services/busy.service';
import { PanelModule } from 'primeng/panel';

@Component({
  selector: 'app-song',
  imports: [ NgClass, PanelModule ],
  templateUrl: './song.component.html',
  styleUrl: './song.component.css'
})
export class SongComponent {
  private songSvc = inject(SongService);
  busySvc = inject(BusyService);

  private isPlayingState = signal<boolean>(false);
  private stepsState = signal<number>(0);

  isPlaying = computed(() => this.isPlayingState());
  currentStep = computed(() => this.stepsState());
  songTitle = this.songSvc.songTitle;

  trackPaths = computed(() => this.songSvc.tracks()?.map(t => 'http://localhost:5244' + t.path));

  howls = computed(() => this.trackPaths()?.map(path => new Howl({ src: [path] })));
  vocals = computed(() => this.howls()?.at(3));
  drums = computed(() => this.howls()?.at(0));
  bass = computed(() => this.howls()?.at(1));
  other = computed(() => this.howls()?.at(2));

  sound!: Howl

  skip = toSignal(this.songSvc.skipGuess$.pipe(tap(() => this.skipGuess())))

  // vocals
  // drums
  // bass
  // other

  stages: { name: string; tracks: string[] }[] = [
    { name: 'Drums', tracks: ['drums'] },
    { name: 'Drums + Bass', tracks: ['drums', 'bass'] },
    { name: 'Drums + Bass + Other', tracks: ['drums', 'bass', 'other'] },
    { name: 'Drums + Bass + Other + Vocals', tracks: ['drums', 'bass', 'other', 'vocals'] }
  ];

  icons = [
    'fa-solid fa-drum',
    'fa-solid fa-guitar',
    'fa-solid fa-music',
    'fa-solid fa-user'
  ]

  constructor() {
    this.songSvc.loadRandomSong$.next(null);

    effect(() => {
      if (!this.isPlaying() && this.howls()) {
        this.howls()!.forEach(h => h.stop());
      }
    });
  }

  playTracks(index: number) {
    if (this.isPlaying() === true) {
      this.setIsPlaying(false);
      this.howls()?.forEach(h => h.stop());
    }

    this.setIsPlaying(true);
    this.stepsState.set(index);

    switch (index) {
      case 0:
        this.drums()?.play();
        break;
      case 1:
        this.drums()?.play();
        this.bass()?.play();
        break;
      case 2:
        this.drums()?.play();
        this.bass()?.play();
        this.other()?.play();
        break;
      default:
        this.howls()?.forEach(h => h.play());
        break;
    }
  }

  setIsPlaying(val: boolean) {
    this.isPlayingState.set(val);
  }

  skipGuess() {
    if (this.isPlaying()) {
      this.setIsPlaying(false);
      this.howls()?.forEach(h => h.stop());
    }
    this.nextStep();
  }

  private nextStep() {
    if (this.currentStep() < 4) {
      this.stepsState.update((state) => state + 1);
    }
  }
}
