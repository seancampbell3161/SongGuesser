import { HttpClient } from "@angular/common/http";
import { computed, inject, Injectable, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { catchError, of, Subject, switchMap } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class GameService {
    private http = inject(HttpClient);

    private isGuessCorrectState = signal<boolean | null>(null);
    isGuessCorrect = computed(() => this.isGuessCorrectState());
    
    sendGuess$ = new Subject<string>();

    constructor() {
        this.sendGuess$.pipe(
            takeUntilDestroyed(),
            switchMap((guess) => this.http.post<boolean>(`http://localhost:5244/api/game`, {userId: '', guess: guess, guessNumber: 1})),
            catchError(() => of(null)),
        ).subscribe(res => this.isGuessCorrectState.set(res));
    }
}