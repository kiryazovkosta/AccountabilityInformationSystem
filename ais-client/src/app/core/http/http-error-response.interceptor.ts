import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";
import { HttpErrorService } from "./http-error.service";
import { SKIP_ERROR_TOAST, SKIP_REFRESH } from "./http-context";

/**
 * Outermost interceptor: sees the final result after any refresh-and-retry.
 * A 401 on a protected request means the session could not be refreshed; AuthService
 * already handled that (toast + redirect), so it is not reported again here.
 */
export const httpErrorResponseInterceptor: HttpInterceptorFn = (req, next) => {
    const httpErrorService = inject(HttpErrorService);

    return next(req).pipe(
        catchError((error) => {
            if (error instanceof HttpErrorResponse) {
                const sessionExpired = error.status === 401 && !req.context.get(SKIP_REFRESH);
                if (!sessionExpired) {
                    httpErrorService.handle(error, { toast: !req.context.get(SKIP_ERROR_TOAST) });
                }
            }
            return throwError(() => error);
        })
    );
};
