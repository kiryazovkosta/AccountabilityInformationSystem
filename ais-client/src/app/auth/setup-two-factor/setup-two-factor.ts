import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { AuthService } from '../../services/shared/auth.service';
import { Router } from '@angular/router';
import { rxResource } from '@angular/core/rxjs-interop';
import { APP_ROUTES } from '../../common/app-routes';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-setup-two-factor',
  imports: [],
  templateUrl: './setup-two-factor.html',
  styleUrl: './setup-two-factor.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SetupTwoFactor {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly setupToken = signal<string | undefined>(history.state?.setupToken);

  readonly setupResource = rxResource({
    params: () => ({ token: this.setupToken() }),
    stream: ({ params }) => this.authService.setup2fa(params.token!)
  });

  code = signal<string>('');
  verifying = signal<boolean>(false);
  verifyError = signal<string | null>(null);
  recoveryCodes = signal<string[]>([]);

  constructor() {
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
