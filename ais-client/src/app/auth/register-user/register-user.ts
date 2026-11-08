import { ChangeDetectionStrategy, Component, inject, input, linkedSignal, signal } from '@angular/core';
import { RegisterUserRequest, RegisterUserFormRequest, toRegisterUserFormRequest, toRegisterUserRequest } from './register-user.request';
import { email, form, maxLength, minLength, required, validate, submit, FormField } from '@angular/forms/signals';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { firstValueFrom } from 'rxjs';
import { APP_ROUTES } from '../../core/app-routes';
import { ToastService } from '../../shared/toast/toast-service';
import { HttpErrorService } from '../../core/http/http-error.service';
import { PASSWORD_HINT, passwordPolicy } from '../../shared/validation/password';

@Component({
  selector: 'app-register-user',
  templateUrl: './register-user.html',
  styleUrl: './register-user.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FormField, RouterLink]
})
export class RegisterUser {

  readonly Paths = APP_ROUTES;
  private readonly router: Router = inject(Router);
  private readonly auth: AuthService = inject(AuthService);
  private readonly toaster = inject(ToastService);
  protected readonly httpErrorService = inject(HttpErrorService);
  protected readonly passwordHint = PASSWORD_HINT;

  registerError = signal<string | null>(null);
  loading = signal<boolean>(false);

  redirectUrl = input<string>('/home');

  registerUser = signal<RegisterUserRequest>({
    username: '',
    email: '',
    firstName: '',
    middleName: undefined,
    lastName: '',
    image: undefined,
    password: '',
    confirmPassword: '',
    enable2Fa: false
  });

  protected readonly registeredUserForm = linkedSignal<RegisterUserFormRequest>(
    () => toRegisterUserFormRequest(this.registerUser())
  );

  registerForm = form(this.registeredUserForm, (schemaPath) => {
    required(schemaPath.username, {message: 'Username is required'});

    required(schemaPath.email, {message: 'Email is required'});
    email(schemaPath.email, {message: 'Enter a valid email address'});

    required(schemaPath.firstName, {message: 'FirstName is required'});
    minLength(schemaPath.firstName, 3, {message: 'FirstName must be at least 3 characters'});
    maxLength(schemaPath.firstName, 32, {message: 'FirstName must be at maximum 32 characters'});

    required(schemaPath.lastName, {message: 'LastName is required'});
    minLength(schemaPath.lastName, 3, {message: 'LastName must be at least 3 characters'});
    maxLength(schemaPath.lastName, 32, {message: 'LastName must be at maximum 32 characters'});

    passwordPolicy(schemaPath.password);

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
      this.registerError.set(null);
      this.httpErrorService.clear();
      try {
        const success = await firstValueFrom(
          this.auth.register(toRegisterUserRequest(this.registerForm().value())));
        if (success) {
          let targetUrl = this.redirectUrl() || APP_ROUTES.HOME;
          if (targetUrl.startsWith('http://') || targetUrl.startsWith('https://')) {
              targetUrl = APP_ROUTES.HOME; 
            }
          this.toaster.show('Successfully register!', 'success');
          this.router.navigateByUrl(targetUrl);
        } else {
          this.registerError.set('Registration failed. Please try again.');
        }
      } catch {
        // The error interceptor records the problem details (shown above the form, no toast).
      } finally {
        this.loading.set(false);
      }
    });
  }
}
