import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, required, email, submit } from '@angular/forms/signals'
import { Router } from '@angular/router';
import { LoginRequest } from './login.model';

@Component({
  selector: 'app-login',
  imports: [FormField],
  templateUrl: './login.html',
  styleUrl: './login.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Login {
  private readonly router = inject(Router);

  loginRequest = signal<LoginRequest>({
    email: '',
    password: '',
    remember: false
  });

  loginForm = form(this.loginRequest, (schemaPath) => {
    required(schemaPath.email, { message: 'Email is required!'});
    email(schemaPath.email, {message: 'Enter a valid email address'});

    required(schemaPath.password, {message: 'Password is required'});
  });

  async onSubmit() {
    await submit(this.loginForm, async () => {
      const credentials = this.loginRequest();
      console.log('Logging in with:', credentials);
      this.router.navigate(['/home']);
    });
  }
}
