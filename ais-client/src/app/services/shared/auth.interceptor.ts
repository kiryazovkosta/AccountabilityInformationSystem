import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);

    if (req.url.includes('/auth/refresh')) {
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
