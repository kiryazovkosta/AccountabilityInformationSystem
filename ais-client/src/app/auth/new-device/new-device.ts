import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { form, FormField, FormRoot, schema, required, FieldTree } from '@angular/forms/signals';
import { FormError } from '../../shared/form-error/form-error';
import { AuthService } from '../../services/shared/auth.service';
import { HttpErrorService } from '../../services/shared/http-error.service';
import { ToastService } from '../../common/toast/toast-service';
import { APP_ROUTES } from '../../common/app-routes';
import { NewDeviceRequest } from './new-device.request';

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
            await firstValueFrom(this.#authService.loginWithRecoveryCode(this.newDeviceRequest()));
            this.#toastService.show('Logged in successfully!', 'success');
            await this.#router.navigate([APP_ROUTES.HOME]);
          } catch {
            // interceptor handles error display
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
