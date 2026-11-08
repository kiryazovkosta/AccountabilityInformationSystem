import { rxResource } from '@angular/core/rxjs-interop';
import { AuthService } from '../../core/auth/auth.service';
import { ChangeDetectionStrategy, Component, effect, inject, input } from '@angular/core';
import { Router } from '@angular/router';
import { APP_ROUTES } from '../../core/app-routes';
import { TwoFactorSetupStore } from '../two-factor-setup.store';

@Component({
  selector: 'app-confirm-email',
  imports: [],
  templateUrl: './confirm-email.html',
  styleUrl: './confirm-email.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmEmail {
  private readonly authService: AuthService = inject(AuthService);
  private readonly router: Router = inject(Router);
  private readonly twoFactorSetupStore = inject(TwoFactorSetupStore);

  userId = input.required<string>();
  code = input.required<string>();

  confirmResource = rxResource({
    params: () => ({ userId: this.userId(), code: this.code() }),
    stream: ({params}) => this.authService.confirmEmail(params.userId, params.code),
  });

  constructor() {
    effect(() => {
      const value = this.confirmResource.value();
      if (value?.requiresTwoFactorSetup && value.setupToken) {
        this.twoFactorSetupStore.set(value.setupToken);
        this.router.navigate([APP_ROUTES.SETUP_2FA]);
      }
    })
  }
}
