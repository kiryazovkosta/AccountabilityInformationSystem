import { HttpClient } from "@angular/common/http";
import { inject, Injectable, signal } from "@angular/core";
import { LoginRequest } from "../../auth/login/login.model";
import { Observable, of } from "rxjs";
import { catchError, map, tap } from "rxjs/operators";
import { LogoutResponse } from "../../auth/logout/logout.response";

@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly httpClient = inject(HttpClient);

    private _isLoggedIn = signal<boolean>(false);

    public isLoggedIn = this._isLoggedIn.asReadonly();

    login(request: LoginRequest): Observable<boolean> {
        return this.httpClient.post("https://localhost:4001/api/identity/auth/login",
            request, { observe: 'response', withCredentials: true })
            .pipe(
                map(response => response.ok),
                tap(success => this._isLoggedIn.set(success))
            );
    }

    logout(): Observable<boolean> {
        return this.httpClient.post<LogoutResponse>("https://localhost:4001/api/identity/auth/logout",
            {},
            { withCredentials: true })
            .pipe(
                map(() => true),
                tap(() => this._isLoggedIn.set(false)),
                catchError(
                    err => {
                        console.error('Logout failed', err);
                        this._isLoggedIn.set(true);
                        return of(false);
                    }
                )
            );
    }

    checkAuth(): Observable<boolean> {
        return this.httpClient.get("https://localhost:4001/api/identity/users/me",
            { observe: 'response', withCredentials: true }
        ).pipe(
            map(response => response.ok),
            tap(valid => this._isLoggedIn.set(valid)),
            catchError(() => of(false))
        );
    }

    refresh(): Observable<boolean> {
        return this.httpClient.post("https://localhost:4001/api/identity/auth/refresh", {},
            { observe: 'response', withCredentials: true }
        ).pipe(
            map(response => response.ok),
            tap(success => this._isLoggedIn.set(success)),
            catchError(() => {
                this._isLoggedIn.set(false);
                return of(false);
            })
        );
    }
}
