import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { NavigationStart, Router } from '@angular/router';
import { filter } from 'rxjs';
import { ToastService } from '../../shared/toast/toast-service';
import { ProblemDetails } from '../../shared/problem-details.model';
import { environment } from '../../../environments/environment';

const DEFAULT_MESSAGES: Record<number, string> = {
    400: 'Invalid request.',
    401: 'Invalid credentials or code.',
    403: 'You do not have permission to perform this action.',
    404: 'Resource not found.',
    409: 'A conflict occurred.',
    500: 'A server error occurred. Please try again later.',
};

@Injectable({ providedIn: 'root' })
export class HttpErrorService {
    private readonly toastService = inject(ToastService);

    readonly currentError = signal<ProblemDetails | null>(null);
    readonly errorDetail = computed(() => {
        const problem = this.currentError();
        if (!problem) return null;
        const fieldMessages = Object.values(problem.errors ?? {}).flat().join(' ');
        return [problem.detail, fieldMessages].filter(Boolean).join(' ') || problem.title || null;
    });
    readonly fieldErrors = computed<Record<string, string>>(() => {
        const errors = this.currentError()?.errors;
        if (!errors) return {};
        return Object.fromEntries(
            Object.entries(errors).map(([key, value]) => [
                key,
                Array.isArray(value) ? value.join(', ') : value,
            ])
        );
    });

    constructor() {
        // An error belongs to the page that caused it: drop it when the user navigates away.
        inject(Router).events
            .pipe(filter(event => event instanceof NavigationStart))
            .subscribe(() => this.clear());
    }

    /**
     * Records the error for inline display and, unless the caller renders it inline,
     * shows a toast.
     */
    handle(error: HttpErrorResponse, options: { toast: boolean } = { toast: true }): void {
        if (!environment.production) {
            console.error(`HTTP ${error.status} ${error.url ?? ''}`);
        }

        const body = error.error as ProblemDetails | null;
        const problem: ProblemDetails =
            body && typeof body === 'object'
                ? body
                : { status: error.status, detail: DEFAULT_MESSAGES[error.status] ?? 'An unexpected error occurred.' };
        this.currentError.set(problem);

        if (options.toast) {
            this.showToast(error.status, problem.detail);
        }
    }

    clear(): void {
        this.currentError.set(null);
    }

    private showToast(status: number, detail: string | undefined): void {
        switch (status) {
            case 400:
                this.toastService.show(detail ?? DEFAULT_MESSAGES[400], 'warning');
                break;
            case 401:
                this.toastService.show('Session expired. Please log in.', 'warning');
                break;
            case 403:
                this.toastService.show(DEFAULT_MESSAGES[403], 'warning');
                break;
            case 404:
                this.toastService.show(detail ?? DEFAULT_MESSAGES[404], 'info');
                break;
            case 409:
                this.toastService.show(detail ?? DEFAULT_MESSAGES[409], 'warning');
                break;
            case 500:
                this.toastService.show(DEFAULT_MESSAGES[500], 'error');
                break;
            default:
                this.toastService.show(detail ?? 'An unexpected error occurred.', 'error');
        }
    }
}
