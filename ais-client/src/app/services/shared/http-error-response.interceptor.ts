import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";
import { APP_ROUTES } from "../../common/app-routes";
import { HttpErrorService } from "./http-error.service";

export const httpErrorResponseInterceptor: HttpInterceptorFn = (req, next) => {
    const httpErrorService = inject(HttpErrorService);
    const router = inject(Router);

    return next(req).pipe(
        catchError((error) => {
            if (error instanceof HttpErrorResponse) {
                httpErrorService.handle(error);
                if (error.status === 401) {
                    router.navigate([APP_ROUTES.LOGIN]);
                }
            }
            return throwError(() => error);
        })
    );
};
