import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { SKIP_REFRESH } from './http-context';

/**
 * On a 401 from a protected endpoint, refreshes the session once (shared by all requests
 * that fail at the same time, see AuthService.refresh) and retries the request.
 * csrfInterceptor runs after this one, so the retry carries the XSRF token issued by the refresh.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
    if (req.context.get(SKIP_REFRESH)) {
        return next(req);
    }

    const authService = inject(AuthService);

    return next(req).pipe(
        catchError(error => {
            if (!(error instanceof HttpErrorResponse) || error.status !== 401) {
                return throwError(() => error);
            }
            return authService.refresh().pipe(
                switchMap(success => (success ? next(req) : throwError(() => error)))
            );
        })
    );
};
