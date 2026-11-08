import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { form, FormField, FormRoot, schema, required, FieldTree } from '@angular/forms/signals';
import { FormError } from '../../shared/form-error/form-error';
import { AuthService } from '../../core/auth/auth.service';
import { HttpErrorService } from '../../core/http/http-error.service';
import { ToastService } from '../../shared/toast/toast-service';
import { APP_ROUTES } from '../../core/app-routes';
import { NewDeviceRequest } from './new-device.request';
import { TwoFactorSetupStore } from '../two-factor-setup.store';

const initialState: NewDeviceRequest = {
  username: '',
  password: '',
  recoveryCode: '',
};

const newDeviceSchema = schema<NewDeviceRequest>((path) => {
  required(path.username, { message: 'Username is required!' });
  required(path.password, { message: 'Password is required!' });
  required(path.recoveryCode, { message: 'Recovery code is required!' });
});

@Component({
  selector: 'app-new-device',
  imports: [FormError, FormField, FormRoot, RouterLink],
  templateUrl: './new-device.html',
  styleUrl: './new-device.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NewDevice {
  readonly #authService = inject(AuthService);
  readonly #router = inject(Router);
  readonly #toastService = inject(ToastService);
  readonly #twoFactorSetupStore = inject(TwoFactorSetupStore);
  protected readonly httpErrorService = inject(HttpErrorService);
  protected readonly Paths = APP_ROUTES;

  protected readonly newDeviceRequest = signal<NewDeviceRequest>(initialState);

  protected readonly newDeviceForm = form(
    this.newDeviceRequest,
    newDeviceSchema,
    {
      submission: {
        action: async () => {
          this.httpErrorService.clear();
          try {
            const result = await firstValueFrom(this.#authService.loginWithRecoveryCode(this.newDeviceRequest()));
            if ('requiresTwoFactorSetup' in result) {
              this.#twoFactorSetupStore.set(result.setupToken);
              await this.#router.navigate([APP_ROUTES.SETUP_2FA]);
            } else {
              this.#toastService.show('Logged in successfully!', 'success');
              await this.#router.navigate([APP_ROUTES.HOME]);
            }
          } catch {
            // The error interceptor records the problem details; the template shows them.
          }
        },
      },
    },
  );

  protected ariaInvalidState(field: FieldTree<unknown>): boolean | undefined {
    return field().touched() && !field().pending()
      ? field().errors().length > 0
      : undefined;
  }
}
