import { Component, computed, effect, inject, input, signal } from '@angular/core';
import { SongService } from './data-access/song.service';
import { Howl } from 'howler';
import { NgClass } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { tap } from 'rxjs';
import { PanelModule } from 'primeng/panel';

@Component({
  selector: 'app-song',
  imports: [ NgClass, PanelModule ],
  templateUrl: './song.component.html',
  styleUrl: './song.component.css'
})
export class SongComponent {
  private songSvc = inject(SongService);

  isDarkMode = input<boolean>();

  private isPlayingState = signal<boolean>(false);

  isPlaying = computed(() => this.isPlayingState());
  currentStep = computed(() => this.songSvc.currentStepState());
  songTitle = this.songSvc.songTitle;

  trackPaths = computed(() => this.songSvc.tracks()?.map(t => 'http://localhost:5244' + t.path));

  private howls = computed(() => this.trackPaths()?.map(path => {
    const sound = new Howl({ 
      src: [path], 
      onload: () => this.duration.set(sound.duration()),
    });
    return sound;
  }));
  private drums = computed(() => this.howls()?.at(0));
  private bass = computed(() => this.howls()?.at(1));
  private other = computed(() => this.howls()?.at(2));

  duration = signal<number>(0);
  currentTime = signal<number>(0);
  widthPercent = computed(() => this.currentTime() / this.duration() * 100);
  intervalId: any;

  sound!: Howl

  skip = toSignal(this.songSvc.skipGuess$.pipe(tap(() => this.skipGuess())));

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
  ];

  constructor() {
    this.songSvc.loadRandomSong$.next(null);

    effect(() => {
      if (!this.isPlaying() && this.howls()) {
        this.reset();
      }
    });

    effect(() => {
      if (this.widthPercent()) {
        const div = document.getElementById(`progress${this.currentStep()}`);

        if (div)
          div.style.width = `${this.widthPercent()}%`;
      }
    })
  }

  playTracks(index: number) {
    if (this.isPlaying() === true) {
      clearInterval(this.intervalId);
      this.setIsPlaying(false);
      this.howls()?.forEach(h => h.stop());
    }

    this.setIsPlaying(true);
    this.songSvc.currentStepState.set(index);

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

    this.intervalId = setInterval(() => { this.currentTime.set(this.drums()!.seek()) }, 100);
  }

  setIsPlaying(val: boolean) {
    this.isPlayingState.set(val);
  }

  skipGuess() {
    if (this.isPlaying()) {
      this.reset();
    }
    this.nextStep();
  }

  private reset() {
    this.setIsPlaying(false);
    this.howls()!.forEach(h => h.stop());
    clearInterval(this.intervalId);
    this.currentTime.set(0);

    const div = document.getElementById(`progress${this.currentStep()}`);

    if (div)
      div.style.width = '0%';
  }

  private nextStep() {
    if (this.currentStep() < 4) {
      this.songSvc.currentStepState.update((state) => state + 1);
    }
  }
}
