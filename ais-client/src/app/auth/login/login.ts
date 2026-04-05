import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, required, email, submit } from '@angular/forms/signals'
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { LoginUserRequest } from './login-user.request';
import { AuthService } from '../../services/shared/auth.service';
import { APP_ROUTES } from '../../common/app-routes';

@Component({
  selector: 'app-login',
  imports: [FormField],
  templateUrl: './login.html',
  styleUrl: './login.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Login {
  private readonly authService: AuthService = inject(AuthService);
  private readonly router: Router = inject(Router);

  loginRequest = signal<LoginUserRequest>({
    username: '',
    password: '',
    remember: false, 
    code: ''
  });

  loginError = signal<string | null>(null);
  loading = signal<boolean>(false);

  loginForm = form(this.loginRequest, (schemaPath) => {
    required(schemaPath.username, { message: 'Username is required!'});
    required(schemaPath.password, {message: 'Password is required'});
  });

  async onSubmit() {
    await submit(this.loginForm, async () => {
      this.loading.set(true);
      this.loginError.set(null);
      try {
        const loginResult = await firstValueFrom(this.authService.login(this.loginRequest()));
        if ('success' in loginResult) {
          this.router.navigate(['/home']);
        } else if ('requiresTwoFactorSetup' in loginResult) {
          this.router.navigate([APP_ROUTES.SETUP_2FA], { state: { setupToken: loginResult.setupToken } });
        }
      } catch {
        this.loginError.set('Invalid email or password.');
      } finally {
        this.loading.set(false);
      }
    });
  }
}
