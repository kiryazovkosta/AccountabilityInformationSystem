import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, required, email, submit } from '@angular/forms/signals'
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { LoginRequest } from './login.model';
import { AuthService } from '../../services/shared/auth.service';

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

  loginRequest = signal<LoginRequest>({
    email: '',
    password: '',
    remember: false
  });

  loginError = signal<string | null>(null);
  loading = signal<boolean>(false);

  loginForm = form(this.loginRequest, (schemaPath) => {
    required(schemaPath.email, { message: 'Email is required!'});
    email(schemaPath.email, {message: 'Enter a valid email address'});

    required(schemaPath.password, {message: 'Password is required'});
  });

  async onSubmit() {
    await submit(this.loginForm, async () => {
      this.loading.set(true);
      this.loginError.set(null);
      try {
        await firstValueFrom(this.authService.login(this.loginRequest()));
        this.router.navigate(['/home']);
      } catch {
        this.loginError.set('Invalid email or password.');
      } finally {
        this.loading.set(false);
      }
    });
  }
}
