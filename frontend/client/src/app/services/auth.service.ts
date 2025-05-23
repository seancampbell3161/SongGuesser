import { computed, effect, inject, Injectable, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { catchError, EMPTY, of, Subject, switchMap } from "rxjs";
import { User } from "../user/user";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
    private http = inject(HttpClient);

    private authToken = signal<string | null>(null);

    isLoggedIn = computed<boolean>(() => this.authToken() != null);

    login$ = new Subject<User>();

    invalidLoginAttempt$ = new Subject<boolean>();

    constructor() {
        const token = localStorage.getItem('token');
        
        if (token) {
            this.authToken.set(token);
        }

        this.login$.pipe(
            takeUntilDestroyed(),
            switchMap((user) => this.http.post<{token: string}>(`http://localhost:5244/api/auth/login`, user)),
            catchError((err: HttpErrorResponse) => {
                if (err.status === 401)
                    this.invalidLoginAttempt$.next(true);
                return EMPTY;
            })
        ).subscribe(res => {
            if (res?.token) {
                localStorage.setItem('token', res.token);
                this.authToken.set(res.token);
                this.invalidLoginAttempt$.next(false);
            }
        });
    }
}