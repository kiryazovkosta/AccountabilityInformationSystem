import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { LoginUserRequest } from '../../auth/login/login-user.request';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { LogoutResponse } from '../../auth/logout/logout.response';
import { RegisterUserRequest } from '../../auth/register-user/register-user.request';
import { environment } from '../../../environments/environment';
import { Endpoints } from '../../common/endpoints-config';

@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly httpClient = inject(HttpClient);

    private _isLoggedIn = signal<boolean>(false);

    public isLoggedIn = this._isLoggedIn.asReadonly();

    login(request: LoginUserRequest): Observable<boolean> {
        return this.httpClient.post(`${environment.apiBaseUrl}${Endpoints.login}`,
            request, { observe: 'response', withCredentials: true })
            .pipe(
                map(response => response.ok),
                tap(success => this._isLoggedIn.set(success))
            );
    }

    register(request: RegisterUserRequest): Observable<boolean> {
        return this.httpClient.post(`${environment.apiBaseUrl}${Endpoints.register}`,
            request, { observe: 'response', withCredentials: true })
            .pipe(
                map(response => response.ok)
            );
    }

    logout(): Observable<boolean> {
        return this.httpClient.post<LogoutResponse>(`${environment.apiBaseUrl}${Endpoints.logout}`,
            {},
            { withCredentials: true })
            .pipe(
                map(() => true),
                tap(() => this._isLoggedIn.set(false)),
                catchError(
                    err => {
                        this._isLoggedIn.set(false);
                        return of(false);
                    }
                )
            );
    }

    checkAuth(): Observable<boolean> {
        return this.httpClient.get(`${environment.apiBaseUrl}${Endpoints.checkAuth}`,
            { observe: 'response', withCredentials: true }
        ).pipe(
            map(response => response.ok),
            tap(valid => this._isLoggedIn.set(valid)),
            catchError(() => of(false))
        );
    }

    refresh(): Observable<boolean> {
        return this.httpClient.post(`${environment.apiBaseUrl}${Endpoints.refresh}`, {},
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
