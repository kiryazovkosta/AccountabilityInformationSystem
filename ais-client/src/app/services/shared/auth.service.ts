import { HttpClient } from "@angular/common/http";
import { inject, Injectable, signal } from "@angular/core";
import { LoginUserRequest } from "../../auth/login/login-user.request";
import { Observable, of } from "rxjs";
import { catchError, map, tap } from "rxjs/operators";
import { LogoutResponse } from "../../auth/logout/logout.response";
import { RegisterUserRequest } from "../../auth/register-user/register-user.request";

@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly httpClient = inject(HttpClient);

    private _isLoggedIn = signal<boolean>(false);

    public isLoggedIn = this._isLoggedIn.asReadonly();

    login(request: LoginUserRequest): Observable<boolean> {
        return this.httpClient.post("https://localhost:4001/api/identity/auth/login",
            request, { observe: 'response', withCredentials: true })
            .pipe(
                map(response => response.ok),
                tap(success => this._isLoggedIn.set(success))
            );
    }

    register(request: RegisterUserRequest): Observable<boolean> {
        return this.httpClient.post("https://localhost:4001/api/identity/auth/register",
            request, { observe: 'response', withCredentials: true })
            .pipe(
                map(response => response.ok)
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
