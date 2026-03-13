import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RegisterUserRequest } from './register-user.request';
import { email, form, FormField, maxLength, minLength, required, SchemaPath, validate, submit } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/shared/auth.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-register-user',
  imports: [FormField, RouterLink],
  templateUrl: './register-user.html',
  styleUrl: './register-user.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegisterUser {
  private readonly router: Router = inject(Router);
  private readonly auth: AuthService = inject(AuthService);

  loginError = signal<string | null>(null);
  loading = signal<boolean>(false);

  registerUser = signal<RegisterUserRequest>({
    email: '',
    firstName: '',
    middleName: undefined,
    lastName: '',
    image: undefined,
    password: '',
    confirmPassword: ''
  });

  registerForm = form(this.registerUser, (schemaPath) => {
    required(schemaPath.email, {message: 'Email is required'});
    email(schemaPath.email, {message: 'Enter a valid email address'});

    required(schemaPath.firstName, {message: 'FirstName is required'});
    minLength(schemaPath.firstName, 3, {message: 'FirstName must be at least 3 characters'});
    maxLength(schemaPath.firstName, 32, {message: 'FirstName must be at maximum 32 characters'});

    validate(schemaPath.middleName as unknown as SchemaPath<string>, ({value}) => {
      const middleName = value();
      if (middleName !== undefined && middleName !== null) {
        if (middleName.length > 32) {
          return {
            kind: 'maxLength',
            message: 'MiddleName must be at maximum 32 characters'
          };
        }
        if (middleName.length < 3) {
          return {
            kind: 'minLength',
            message: 'MiddleName must be at least 3 characters'
          };
        }
        return null;
      }
      return null;
    });

    required(schemaPath.lastName, {message: 'LastName is required'});
    minLength(schemaPath.lastName, 3, {message: 'LastName must be at least 3 characters'});
    maxLength(schemaPath.lastName, 32, {message: 'LastName must be at maximum 32 characters'});

    required(schemaPath.password, {message: 'Password is required'});
    minLength(schemaPath.password, 6, {message: 'Password must be at least 8 characters'});

    required(schemaPath.confirmPassword, {message: 'Confirm password is required'});
    validate(schemaPath.confirmPassword, ({value, valueOf}) => {
      const confirmPassword = value();
      const password = valueOf(schemaPath.password);

      if (password !== confirmPassword) {
        return {
          kind: 'passwordMismatch',
          message: 'Passwords do not match'
        };
      }

      return null;
    });
  });

  async onSubmit() {
    await submit(this.registerForm, async() => {
      this.loading.set(true);
      this.loginError.set(null);
      try {
        await firstValueFrom(this.auth.register(this.registerUser()));
        this.router.navigate(['/auth/login']);
      } catch {
        this.loginError.set('Failed to registed user.');
      } finally {
        this.loading.set(false);
      }
    });
  }
}
