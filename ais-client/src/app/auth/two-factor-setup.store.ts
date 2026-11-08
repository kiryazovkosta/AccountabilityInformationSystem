import { Injectable, signal } from '@angular/core';

/**
 * Holds the short-lived 2FA setup token in memory only, so it never ends up in
 * browser session history (history.state is persisted for back/forward and session restore).
 */
@Injectable({ providedIn: 'root' })
export class TwoFactorSetupStore {
  readonly #token = signal<string | undefined>(undefined);
  readonly token = this.#token.asReadonly();

  set(token: string): void {
    this.#token.set(token);
  }

  clear(): void {
    this.#token.set(undefined);
  }
}
