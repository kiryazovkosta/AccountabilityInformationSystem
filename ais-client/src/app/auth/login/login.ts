import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, required, submit } from '@angular/forms/signals'
import { Router, RouterLink } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { LoginUserRequest } from './login-user.request';
import { AuthService } from '../../services/shared/auth.service';
import { HttpErrorService } from '../../services/shared/http-error.service';
import { APP_ROUTES } from '../../common/app-routes';

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
  protected readonly httpErrorService = inject(HttpErrorService);

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
          this.router.navigate(['/home']);
        } else if ('requiresTwoFactorSetup' in loginResult) {
          this.router.navigate([APP_ROUTES.SETUP_2FA], { state: { setupToken: loginResult.setupToken } });
        }
      } catch {
        // interceptor already handled the error and showed a toast
      } finally {
        this.loading.set(false);
      }
    });
  }
}
