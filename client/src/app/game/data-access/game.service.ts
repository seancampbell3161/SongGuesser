import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { catchError, EMPTY, of, Subject, switchMap } from "rxjs";
import { SongService } from "../../song/data-access/song.service";
import { UserGuess } from "../interfaces/user-guess";
import { UserScore } from "../../user/user-score";

@Injectable({
  providedIn: 'root'
})
export class GameService {
    private http = inject(HttpClient);
    private songSvc = inject(SongService);

    isGuessCorrect$ = new Subject<boolean>();
    sendGuess$ = new Subject<string>();
    gameWon$ = new Subject<boolean>();

    loadLeaderboard$ = this.http.get<UserScore[]>(`http://localhost:5244/api/Game/leaderboard`).pipe(
        takeUntilDestroyed(),
        catchError(() => EMPTY)
    );

    constructor() {
        this.sendGuess$.pipe(
            takeUntilDestroyed(),
            switchMap((guess) => this.http.post<boolean>(`http://localhost:5244/api/Game`, { guess: guess, songId: this.songSvc.song()?.songId } as UserGuess)),
        ).subscribe({
            next: (res) => this.isGuessCorrect$.next(!!res),
            error: (err) => console.error(err)
        });
    }
}