import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, required, submit } from '@angular/forms/signals';
import { RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { ForgotPasswordRequest } from './forgot-password.request';
import { AuthService } from '../../services/shared/auth.service';
import { HttpErrorService } from '../../services/shared/http-error.service';
import { APP_ROUTES } from '../../common/app-routes';

@Component({
  selector: 'app-forgot-password',
  imports: [FormField, RouterLink],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ForgotPassword {
  private readonly authService = inject(AuthService);
  protected readonly httpErrorService = inject(HttpErrorService);
  readonly Paths = APP_ROUTES;

  protected readonly request = signal<ForgotPasswordRequest>({ username: '' });
  protected readonly loading = signal(false);
  protected readonly submitted = signal(false);

  protected readonly forgotForm = form(this.request, (p) => {
    required(p.username, { message: 'Username is required.' });
  });

  async onSubmit() {
    this.httpErrorService.clear();
    await submit(this.forgotForm, async () => {
      this.loading.set(true);
      try {
        await firstValueFrom(this.authService.forgotPassword(this.request().username));
        this.submitted.set(true);
      } catch {
        // interceptor handles error display
      } finally {
        this.loading.set(false);
      }
    });
  }
}
