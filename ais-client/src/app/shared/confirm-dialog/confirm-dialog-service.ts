import { Injectable, signal } from '@angular/core';
import { ConfirmDialogOptions, ConfirmDialogRequest } from './confirm-dialog.model';

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  request = signal<ConfirmDialogRequest | null>(null);
  #resolve: ((confirmed: boolean) => void) | null = null;

  confirm(options: ConfirmDialogOptions): Promise<boolean> {
    this.#settle(false);

    return new Promise<boolean>((resolve) => {
      this.#resolve = resolve;
      this.request.set({
        title: options.title,
        message: options.message,
        confirmText: options.confirmText ?? 'Confirm',
        cancelText: options.cancelText ?? 'Cancel',
        variant: options.variant ?? 'default',
      });
    });
  }

  onConfirm(): void {
    this.#settle(true);
  }

  onCancel(): void {
    this.#settle(false);
  }

  #settle(confirmed: boolean): void {
    this.#resolve?.(confirmed);
    this.#resolve = null;
    this.request.set(null);
  }
}
