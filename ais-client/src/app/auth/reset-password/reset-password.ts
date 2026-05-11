import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { form, FormField, required, submit, validate } from '@angular/forms/signals';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ResetPasswordFormData } from './reset-password.request';
import { AuthService } from '../../services/shared/auth.service';
import { HttpErrorService } from '../../services/shared/http-error.service';
import { APP_ROUTES } from '../../common/app-routes';

@Component({
  selector: 'app-reset-password',
  imports: [FormField],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ResetPassword {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly httpErrorService = inject(HttpErrorService);

  userId = input.required<string>();
  code = input.required<string>();

  protected readonly loading = signal(false);
  protected readonly formData = signal<ResetPasswordFormData>({ newPassword: '', confirmPassword: '' });

  protected readonly resetForm = form(this.formData, (p) => {
    required(p.newPassword, { message: 'New password is required.' });
    required(p.confirmPassword, { message: 'Please confirm your password.' });
    validate(p.confirmPassword, ({ value, valueOf }) => {
      if (value() !== valueOf(p.newPassword)) {
        return { kind: 'passwordMismatch', message: 'Passwords do not match.' };
      }
      return null;
    });
  });

  async onSubmit() {
    this.httpErrorService.clear();
    await submit(this.resetForm, async () => {
      this.loading.set(true);
      try {
        await firstValueFrom(this.authService.resetPassword({
          userId: this.userId(),
          code: this.code(),
          newPassword: this.formData().newPassword,
          confirmPassword: this.formData().confirmPassword
        }));
        this.router.navigate([APP_ROUTES.LOGIN]);
      } catch {
        // interceptor handles error display
      } finally {
        this.loading.set(false);
      }
    });
  }
}
