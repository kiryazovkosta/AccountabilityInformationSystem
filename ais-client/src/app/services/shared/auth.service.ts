import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { rxResource } from '@angular/core/rxjs-interop';
import { LoginUserRequest } from '../../auth/login/login-user.request';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { LogoutResponse } from '../../auth/logout/logout.response';
import { RegisterUserRequest } from '../../auth/register-user/register-user.request';
import { environment } from '../../../environments/environment';
import { Endpoints } from '../../common/endpoints-config';
import { ResendEmailConfirmationRequest } from '../../auth/resend-email-confirmation/resend-email-confirmation-request';

export interface ConfirmEmailResponse {
  requiresTwoFactorSetup: boolean;
  setupToken?: string;
}

export interface Setup2faResponse {
  qrCodeBase64: string;
  manualEntryKey: string;
}

export interface Verify2faResponse {
  recoveryCodes: string[];
}

export type LoginResult =
  | { success: true }
  | { requiresTwoFactorSetup: true; setupToken: string };

interface LoginTwoFactorSetupRequired {
    requiresTwoFactorSetup: true;
    setupToken: string;
}

@Injectable({providedIn: 'root'})
export class AuthService {
    private readonly httpClient = inject(HttpClient);

    private _isLoggedIn = signal<boolean>(false);

    public isLoggedIn = this._isLoggedIn.asReadonly();

    private resendPayload = signal<ResendEmailConfirmationRequest | undefined>(undefined);

    readonly resendEmailConfirmation = rxResource({
        params: () => this.resendPayload(),
        stream: ({ params }) =>
            this.httpClient.get(
                `${environment.apiBaseUrl}${Endpoints.resendConfirmationEmail}`,
                { params: { ...params }, withCredentials: true }
            )
    });

    register(request: RegisterUserRequest): Observable<boolean> {
        return this.httpClient.post(`${environment.apiBaseUrl}${Endpoints.register}`,
            request, { observe: 'response', withCredentials: true })
            .pipe(
                map(response => response.ok)
            );
    }

    confirmEmail(userId: string, code: string): Observable<ConfirmEmailResponse> {
        return this.httpClient.get<ConfirmEmailResponse>(
            `${environment.apiBaseUrl}${Endpoints.confirmEmail}`,
            { params: { userId, code }, withCredentials: true }
        );
    }

    resendEmail(request: ResendEmailConfirmationRequest) {
        this.resendPayload.set(request);
    }

    setup2fa(setupToken: string): Observable<Setup2faResponse> {
        return this.httpClient.post<Setup2faResponse>(
            `${environment.apiBaseUrl}${Endpoints.setup2fa}`,
            { setupToken },
            { withCredentials: true }
        );
    }

    verify2fa(setupToken: string, code: string): Observable<Verify2faResponse> {
        return this.httpClient.post<Verify2faResponse>(
            `${environment.apiBaseUrl}${Endpoints.verify2fa}`,
            { setupToken, code },
            { withCredentials: true }
        );
    }

    login(request: LoginUserRequest): Observable<LoginResult> {
        return this.httpClient.post<LoginTwoFactorSetupRequired>(
            `${environment.apiBaseUrl}${Endpoints.login}`,
            request, 
            { observe: 'response', withCredentials: true }
        )
        .pipe(
            map(response => {
                if (response.status === 202 && response.body?.requiresTwoFactorSetup) {
                    return { requiresTwoFactorSetup: true, setupToken: response.body.setupToken } as LoginResult;
                }
                this._isLoggedIn.set(true);
                return { success: true } as LoginResult;
            })
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
