import { AfterViewInit, Component, OnInit, computed, effect, inject, signal } from '@angular/core';
import { SongService } from './data-access/song.service';
import { Howl } from 'howler';

@Component({
  selector: 'app-song',
  imports: [],
  templateUrl: './song.component.html',
  styleUrl: './song.component.css'
})
export class SongComponent {
  private songSvc = inject(SongService);

  private isPlayingState = signal<boolean>(false);
  private stepsState = signal<number>(1);

  isPlaying = computed(() => this.isPlayingState());
  currentStep = computed(() => this.stepsState());
  songTitle = this.songSvc.songTitle;
  trackPaths = computed(() => this.songSvc.tracks()?.map(t => 'http://localhost:5244' + t.path));
  howls = computed(() => this.trackPaths()?.map(path => new Howl({ src: [path] })));
  vocals = computed(() => new Howl({ src: this.trackPaths() ? this.trackPaths()![0] : ''}));
  drums = computed(() => new Howl({ src: this.trackPaths() ? this.trackPaths()![1] : ''}));
  bass = computed(() => new Howl({ src: this.trackPaths() ? this.trackPaths()![2] : ''}));
  other = computed(() => new Howl({ src: this.trackPaths() ? this.trackPaths()![3] : ''}));

  sound!: Howl

  // vocals
  // drums
  // bass
  // other

  stages = [
    { name: 'Drums', tracks: ['drums'] },
    { name: 'Drums + Bass', tracks: ['drums', 'bass'] },
    { name: 'Drums + Bass + Other', tracks: ['drums', 'bass', 'other'] },
    { name: 'Drums + Bass + Other + Vocals', tracks: ['drums', 'bass', 'other', 'vocals'] }
  ];

  constructor() {
    this.songSvc.loadRandomSong$.next(null);

    effect(() => {
      if (this.isPlaying() && this.trackPaths()) {
        console.log(this.trackPaths());
        this.drums().play();
        this.bass().play();
        this.other().play();

      } else if (!this.isPlaying() && this.trackPaths()) {
        this.drums().stop();
        this.bass().stop();
        this.other().stop();
        this.vocals().stop();
      }
    })
  }

  setIsPlaying(val: boolean) {
    this.isPlayingState.set(val);
  }

  nextStep() {
    if (this.currentStep() < 4) {
      this.stepsState.update((state) => state + 1);
    }
  }
}
