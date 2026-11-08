import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { rxResource } from '@angular/core/rxjs-interop';
import { LoginUserRequest } from '../../auth/login/login-user.request';
import { Observable, of } from 'rxjs';
import { catchError, finalize, map, shareReplay, switchMap, tap } from 'rxjs/operators';
import { anonymousRequest, inlineErrors } from '../http/http-context';
import { ToastService } from '../../shared/toast/toast-service';
import { APP_ROUTES } from '../app-routes';
import { RegisterUserRequest } from '../../auth/register-user/register-user.request';
import { environment } from '../../../environments/environment';
import { Endpoints } from '../http/endpoints-config';
import { ResendEmailConfirmationRequest } from '../../auth/resend-email-confirmation/resend-email-confirmation-request';
import { ResetPasswordRequest } from '../../auth/reset-password/reset-password.request';
import { ChangePasswordRequest } from '../../auth/change-password/change-password';
import { NewDeviceRequest } from '../../auth/new-device/new-device.request';

export interface UserResponse {
  id: string;
  username: string;
  email: string;
  fullName: string;
  image: string | null;
  createdAt: string;
  modifiedAt: string;
  roles?: string[];
}

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
    private readonly router = inject(Router);
    private readonly toastService = inject(ToastService);

    /** The refresh call currently in flight, shared by every request that hit a 401 at the same time. */
    private refreshInFlight$: Observable<boolean> | null = null;

    private _isLoggedIn = signal<boolean>(false);
    private _currentUser = signal<UserResponse | null>(null);

    public isLoggedIn = this._isLoggedIn.asReadonly();
    public currentUser = this._currentUser.asReadonly();

    /** Id of the signed-in user, or null. Data resources key on it so they reset on logout. */
    public userId = computed(() => this._currentUser()?.id ?? null);

    public isAdministrator = computed(() =>
        this._currentUser()?.roles?.some(r => r.toLowerCase() === 'admin') ?? false
    );

    private resendPayload = signal<ResendEmailConfirmationRequest | undefined>(undefined);

    readonly resendEmailConfirmation = rxResource({
        params: () => this.resendPayload(),
        stream: ({ params }) =>
            this.httpClient.get(
                `${environment.apiBaseUrl}${Endpoints.resendConfirmationEmail}`,
                { params: { email: params.email }, context: anonymousRequest() }
            )
    });

    register(request: RegisterUserRequest): Observable<boolean> {
        return this.httpClient.post(`${environment.apiBaseUrl}${Endpoints.register}`,
            request, { observe: 'response', context: inlineErrors(anonymousRequest()) })
            .pipe(
                map(response => response.ok)
            );
    }

    confirmEmail(userId: string, code: string): Observable<ConfirmEmailResponse> {
        return this.httpClient.get<ConfirmEmailResponse>(
            `${environment.apiBaseUrl}${Endpoints.confirmEmail}`,
            { params: { userId, code }, context: anonymousRequest() }
        );
    }

    resendEmail(request: ResendEmailConfirmationRequest) {
        this.resendPayload.set(request);
    }

    setup2fa(setupToken: string): Observable<Setup2faResponse> {
        return this.httpClient.post<Setup2faResponse>(
            `${environment.apiBaseUrl}${Endpoints.setup2fa}`,
            { setupToken },
            { context: anonymousRequest() }
        );
    }

    verify2fa(setupToken: string, code: string): Observable<Verify2faResponse> {
        return this.httpClient.post<Verify2faResponse>(
            `${environment.apiBaseUrl}${Endpoints.verify2fa}`,
            { setupToken, code },
            { context: anonymousRequest() }
        );
    }

    login(request: LoginUserRequest): Observable<LoginResult> {
        return this.httpClient.post<LoginTwoFactorSetupRequired>(
            `${environment.apiBaseUrl}${Endpoints.login}`,
            request,
            { observe: 'response', context: inlineErrors(anonymousRequest()) }
        ).pipe(
            switchMap(response => {
                if (response.status === 202 && response.body?.requiresTwoFactorSetup) {
                    return of({ requiresTwoFactorSetup: true, setupToken: response.body.setupToken } as LoginResult);
                }
                return this.checkAuth().pipe(map(() => ({ success: true } as LoginResult)));
            })
        );
    }

    logout(): Observable<boolean> {
        return this.httpClient.post<void>(`${environment.apiBaseUrl}${Endpoints.logout}`,
            {},
            { context: anonymousRequest() })
            .pipe(
                map(() => true),
                catchError(() => of(false)),
                // Clear the local session whether or not the server call succeeded.
                finalize(() => this.clearSession())
            );
    }

    checkAuth(): Observable<boolean> {
        return this.httpClient.get<UserResponse>(`${environment.apiBaseUrl}${Endpoints.checkAuth}`,
            { observe: 'response' }
        ).pipe(
            tap(response => {
                if (response.ok && response.body) {
                    this._currentUser.set(response.body);
                    this._isLoggedIn.set(true);
                } else {
                    this._currentUser.set(null);
                    this._isLoggedIn.set(false);
                }
            }),
            map(response => response.ok),
            catchError(() => {
                this._isLoggedIn.set(false);
                this._currentUser.set(null);
                return of(false);
            })
        );
    }

    forgotPassword(username: string): Observable<void> {
        return this.httpClient.post<void>(
            `${environment.apiBaseUrl}${Endpoints.forgotPassword}`,
            { username },
            { context: inlineErrors(anonymousRequest()) }
        );
    }

    resetPassword(request: ResetPasswordRequest): Observable<void> {
        return this.httpClient.post<void>(
            `${environment.apiBaseUrl}${Endpoints.resetPassword}`,
            request,
            { context: inlineErrors(anonymousRequest()) }
        );
    }

    /**
     * Refreshes the session. Concurrent callers share one request: the server rotates the
     * refresh token, so a second parallel refresh would present a revoked token and fail.
     */
    refresh(): Observable<boolean> {
        if (this.refreshInFlight$) {
            return this.refreshInFlight$;
        }

        const wasLoggedIn = this._isLoggedIn();
        this.refreshInFlight$ = this.httpClient.post(`${environment.apiBaseUrl}${Endpoints.refresh}`, {},
            { observe: 'response', context: anonymousRequest() }
        ).pipe(
            map(response => response.ok),
            catchError(() => of(false)),
            tap(success => {
                if (success) {
                    this._isLoggedIn.set(true);
                    return;
                }
                this.clearSession();
                if (wasLoggedIn) {
                    this.onSessionExpired();
                }
            }),
            finalize(() => (this.refreshInFlight$ = null)),
            shareReplay({ bufferSize: 1, refCount: false })
        );
        return this.refreshInFlight$;
    }

    changePassword(request: ChangePasswordRequest): Observable<void> {
        return this.httpClient.post<void>(
            `${environment.apiBaseUrl}${Endpoints.changePassword}`,
            request,
            { context: inlineErrors() }
        );
    }

    loginWithRecoveryCode(request: NewDeviceRequest): Observable<LoginResult> {
        return this.httpClient.post<LoginTwoFactorSetupRequired>(
            `${environment.apiBaseUrl}${Endpoints.newDevice2fa}`,
            request,
            { observe: 'response', context: inlineErrors(anonymousRequest()) }
        ).pipe(
            switchMap(response => {
                if (response.status === 202 && response.body?.requiresTwoFactorSetup) {
                    return of({ requiresTwoFactorSetup: true, setupToken: response.body.setupToken } as LoginResult);
                }
                return this.checkAuth().pipe(map(() => ({ success: true } as LoginResult)));
            })
        );
    }

    private clearSession(): void {
        this._isLoggedIn.set(false);
        this._currentUser.set(null);
    }

    private onSessionExpired(): void {
        this.toastService.show('Session expired. Please log in.', 'warning');
        const returnUrl = this.router.url;
        this.router.navigate([APP_ROUTES.LOGIN], {
            queryParams: returnUrl && returnUrl !== APP_ROUTES.LOGIN ? { returnUrl } : undefined,
        });
    }
}
