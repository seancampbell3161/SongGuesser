import { Injectable, computed, inject, signal } from '@angular/core';
import { Song } from '../interfaces/song';
import { EMPTY, Subject, catchError, of, switchMap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class SongService {
  private http = inject(HttpClient);

  private songState = signal<Song | null>(null);
  currentStepState = signal<number>(0);

  song = computed(() => this.songState());
  songTitle = computed(() => this.songState()?.title);
  artist = computed(() => this.songState()?.artistName);
  tracks = computed(() => this.songState()?.tracks);

  loadSongOfTheDay$ = new Subject<void>();
  loadRandomSong$ = new Subject<void>();
  skipGuess$ = new Subject<void>();

  constructor() {
    this.loadSongOfTheDay$.pipe(
      takeUntilDestroyed(),
      switchMap(() => this.http.get<Song>(`http://localhost:5244/api/Music/song`).pipe(
        catchError((err) => {
          console.error(err);
          return EMPTY;
        })
      )),
    ).subscribe((res) => 
      this.songState.set(res)
    );

    this.loadRandomSong$.pipe(
      takeUntilDestroyed(),
      switchMap(() => this.http.get<Song>(`http://localhost:5244/api/Music/random`)),
      catchError(() => of(null)),
    ).subscribe((res) => 
      this.songState.set(res)
    );
  }
}
