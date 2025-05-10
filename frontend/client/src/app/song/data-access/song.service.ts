import { Injectable, computed, inject, signal } from '@angular/core';
import { Song } from '../interfaces/song';
import { Subject, catchError, of, switchMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SongService {
  private http = inject(HttpClient);

  private songState = signal<Song | null>(null);

  song = computed(() => this.songState());
  songTitle = computed(() => this.songState()?.title);
  artist = computed(() => this.songState()?.artistName);
  tracks = computed(() => this.songState()?.tracks);

  loadRandomSong$ = new Subject<null>();
  skipGuess$ = new Subject<void>();

  constructor() {
    this.loadRandomSong$.pipe(
      takeUntilDestroyed(),
      switchMap(() => this.http.get<Song>(`http://localhost:5244/api/Music/random`)),
      catchError(() => of(null)),
    ).subscribe((res) => this.songState.set(res));
  }
}
