import { HttpClient } from "@angular/common/http";
import { computed, inject, Injectable, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { catchError, of, Subject, switchMap } from "rxjs";
import { SongService } from "../../song/data-access/song.service";
import { UserGuess } from "../interfaces/user-guess";

@Injectable({
  providedIn: 'root'
})
export class GameService {
    private http = inject(HttpClient);
    private songSvc = inject(SongService);

    isGuessCorrect$ = new Subject<boolean>();
    
    sendGuess$ = new Subject<string>();

    constructor() {
        this.sendGuess$.pipe(
            takeUntilDestroyed(),
            switchMap((guess) => this.http.post<boolean>(`http://localhost:5244/api/Game`, { guess: guess, guessNumber: this.songSvc.currentStepState() } as UserGuess)),
            catchError(() => of(null)),
        ).subscribe(res => 
            this.isGuessCorrect$.next(!!res)
        );
    }
}