import { ChangeDetectionStrategy, Component, inject, input, signal } from '@angular/core';
import { form, FormField, required, submit } from '@angular/forms/signals'
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { LoginUserRequest } from './login-user.request';
import { AuthService } from '../../core/auth/auth.service';
import { HttpErrorService } from '../../core/http/http-error.service';
import { APP_ROUTES } from '../../core/app-routes';
import { TwoFactorSetupStore } from '../two-factor-setup.store';

@Component({
  selector: 'app-login',
  imports: [FormField, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Login {
  private readonly authService: AuthService = inject(AuthService);
  private readonly router: Router = inject(Router);
  private readonly twoFactorSetupStore = inject(TwoFactorSetupStore);
  protected readonly httpErrorService = inject(HttpErrorService);

  /** Where to go after logging in (`?returnUrl=` set by the guards and on session expiry). */
  readonly returnUrl = input<string | undefined>(undefined);

  loginRequest = signal<LoginUserRequest>({
    username: '',
    password: '',
    remember: false,
    code: ''
  });

  loading = signal<boolean>(false);

  loginForm = form(this.loginRequest, (schemaPath) => {
    required(schemaPath.username, { message: 'Username is required!'});
    required(schemaPath.password, {message: 'Password is required'});
  });

  readonly Paths = APP_ROUTES;

  async onSubmit() {
    this.httpErrorService.clear();
    await submit(this.loginForm, async () => {
      this.loading.set(true);
      try {
        const loginResult = await firstValueFrom(this.authService.login(this.loginRequest()));
        if ('success' in loginResult) {
          this.router.navigateByUrl(this.safeReturnUrl());
        } else if ('requiresTwoFactorSetup' in loginResult) {
          this.twoFactorSetupStore.set(loginResult.setupToken);
          this.router.navigate([APP_ROUTES.SETUP_2FA]);
        }
      } catch {
        // The error interceptor records the problem details (including a 401 for wrong
        // credentials) and the template shows them above the form.
      } finally {
        this.loading.set(false);
      }
    });
  }

  /** Only same-app paths, never an absolute or protocol-relative URL (open redirect). */
  private safeReturnUrl(): string {
    const url = this.returnUrl();
    return url && url.startsWith('/') && !url.startsWith('//') ? url : APP_ROUTES.HOME;
  }
}
