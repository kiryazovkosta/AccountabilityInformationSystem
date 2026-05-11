import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { catchError, throwError } from "rxjs";
import { HttpErrorService } from "./http-error.service";
import { inject } from "@angular/core";

export const httpErrorResponseInterceptor: HttpInterceptorFn = (req, next) => {
    const httpErrorService = inject(HttpErrorService);

    return next(req).pipe(
        catchError((error) => {
            if (error instanceof HttpErrorResponse && error.status !== 401) {
                httpErrorService.handle(error);
            }
            return throwError(() => error);
        })
    );
};
