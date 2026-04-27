import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastService } from '../../common/toast/toast-service';
import { ProblemDetails } from '../../shared/problem-details.model';

@Injectable({ providedIn: 'root' })
export class HttpErrorService {
    private readonly toastService = inject(ToastService);

    readonly currentError = signal<ProblemDetails | null>(null);
    readonly errorDetail = computed(() => this.currentError()?.detail ?? null);
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

    handle(error: HttpErrorResponse): void {
        const problem = error.error as ProblemDetails | null;
        this.currentError.set(problem ?? null);

        const detail = problem?.detail;

        switch (error.status) {
            case 400:
                this.toastService.show(detail ?? 'Invalid request.', 'warning');
                break;
            case 401:
                this.toastService.show('Session expired. Please log in.', 'warning');
                break;
            case 403:
                this.toastService.show('You do not have permission to perform this action.', 'warning');
                break;
            case 404:
                this.toastService.show(detail ?? 'Resource not found.', 'info');
                break;
            case 409:
                this.toastService.show(detail ?? 'A conflict occurred.', 'warning');
                break;
            case 500:
                this.toastService.show('A server error occurred. Please try again later.', 'error');
                break;
            default:
                this.toastService.show(detail ?? 'An unexpected error occurred.', 'error');
        }
    }

    clear(): void {
        this.currentError.set(null);
    }
}
