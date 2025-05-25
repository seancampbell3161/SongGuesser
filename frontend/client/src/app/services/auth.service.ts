import { computed, effect, inject, Injectable, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { catchError, EMPTY, EmptyError, of, Subject, switchMap } from "rxjs";
import { User } from "../user/user";
import { HttpClient, HttpErrorResponse } from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class AuthService {
    private http = inject(HttpClient);

    private apiUrl = 'http://localhost:5244/api'

    private authToken = signal<string | null>(null);

    isLoggedIn = computed<boolean>(() => this.authToken() != null);

    login$ = new Subject<User>();
    register$ = new Subject<User>();

    invalidLoginAttempt$ = new Subject<boolean>();
    invalidRegisterAttempt$ = new Subject<{ code: string, description: string }[]>();

    constructor() {
        const token = localStorage.getItem('token');
        
        if (token) {
            this.authToken.set(token);
        }

        this.login$.pipe(
            takeUntilDestroyed(),
            switchMap((user) => this.http.post<{token: string}>(`${this.apiUrl}/auth/login`, user)),
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

        this.register$.pipe(
            takeUntilDestroyed(),
            switchMap((user) => this.http.post<{ token: string }>(`${this.apiUrl}/auth/register`, user).pipe(
                catchError((err: HttpErrorResponse) => {
                    if (err.status === 400)
                    this.invalidRegisterAttempt$.next(err.error);
                return EMPTY;
                })
            )),
        ).subscribe(res => {
            if (res?.token) {
                localStorage.setItem('token', res.token);
                this.authToken.set(res.token);
                this.invalidLoginAttempt$.next(false);
            }
        })
    }
}