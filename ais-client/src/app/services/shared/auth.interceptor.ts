import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);

    // Logout is anonymous on the server, so it never needs a refreshed access token
    if (req.url.includes('/auth/refresh') || req.url.includes('/auth/logout')) {
        return next(req);
    }

    return next(req).pipe(
        catchError(error => {
            if (error.status !== 401) {
                return throwError(() => error);
            }
            return authService.refresh().pipe(
                switchMap(success => {
                    if (success) {
                        return next(req);
                    }
                    return throwError(() => error);
                })
            );
        })
    );
};
