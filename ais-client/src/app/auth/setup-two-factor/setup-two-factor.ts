import { ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit, signal } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';
import { Router } from '@angular/router';
import { rxResource } from '@angular/core/rxjs-interop';
import { APP_ROUTES } from '../../core/app-routes';
import { firstValueFrom } from 'rxjs';
import { TwoFactorSetupStore } from '../two-factor-setup.store';

@Component({
  selector: 'app-setup-two-factor',
  imports: [],
  templateUrl: './setup-two-factor.html',
  styleUrl: './setup-two-factor.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SetupTwoFactor implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly setupStore = inject(TwoFactorSetupStore);

  /** Kept in memory only (never in history.state), and cleared when the page is left. */
  readonly setupToken = this.setupStore.token;

  readonly setupResource = rxResource({
    // Undefined params keep the resource idle, so no request is sent without a token.
    params: () => {
      const token = this.setupToken();
      return token ? { token } : undefined;
    },
    stream: ({ params }) => this.authService.setup2fa(params.token)
  });

  code = signal<string>('');
  verifying = signal<boolean>(false);
  verifyError = signal<string | null>(null);
  recoveryCodes = signal<string[]>([]);

  constructor() {
    inject(DestroyRef).onDestroy(() => this.setupStore.clear());
  }

  ngOnInit(): void {
    if (!this.setupToken()) {
      this.router.navigate([APP_ROUTES.LOGIN]);
    }
  }

  async onVerify() {
    const token = this.setupToken();
    const currentCode = this.code();
    if (!token || !currentCode) return;

    this.verifying.set(true);
    this.verifyError.set(null);
    try {
      const result = await firstValueFrom(this.authService.verify2fa(token, currentCode));
      this.recoveryCodes.set(result.recoveryCodes);
      this.setupStore.clear();
    } catch {
      this.verifyError.set('Invalid code. Please try again.');
    } finally {
      this.verifying.set(false);
    }
  }

  goToLogin() {
    this.router.navigate([APP_ROUTES.LOGIN]);
  }
}
