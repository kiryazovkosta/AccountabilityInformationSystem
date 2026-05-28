import { Component, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { form, FormField, schema, FormRoot, required, FieldTree, validate } from '@angular/forms/signals';
import { FormError } from "../../shared/form-error/form-error";
import { AuthService } from '../../services/shared/auth.service';
import { HttpErrorService } from '../../services/shared/http-error.service';

export interface ChangePasswordRequest {
  oldPassword: string,
  newPassword: string,
  confirmPassword: string,
  code: string,
}

const initialState: ChangePasswordRequest = {
  oldPassword: '',
  newPassword: '',
  confirmPassword: '',
  code: ''
}

export const changePasswordSchema = schema<ChangePasswordRequest>(
  (path) => {
    required(path.oldPassword, { message: 'Current password is required!'});
    required(path.newPassword, { message: 'New password is required!'});
    required(path.confirmPassword, { message: 'Confirming of new password is required!'});
    validate(path.confirmPassword, ({value, valueOf}) => {
      const confirmPassword = value();
      const newPassword = valueOf(path.newPassword);
      if (confirmPassword !== newPassword) {
        return {
          kind: "passwordMismatch",
          message: "Passwords do not match",
        };
      }

      return null;
    })
  });

@Component({
  selector: 'app-change-password',
  imports: [FormError, FormField, FormRoot ],
  templateUrl: './change-password.html',
  styleUrl: './change-password.css',
})
export class ChangePassword {
  readonly #authService = inject(AuthService);
  protected readonly httpErrorService = inject(HttpErrorService);
  protected readonly changePasswordRequest = signal<ChangePasswordRequest>(initialState);
  protected readonly changePasswordForm = form(
    this.changePasswordRequest,
    changePasswordSchema,
    {
      submission: {
        action: async (form) => {
          this.httpErrorService.clear();
          try {
            await firstValueFrom(this.#authService.changePassword(this.changePasswordForm().value()));
            this.resetForm();
          } catch {
            // interceptor handles error display
          }
        }
      }
    }
  );

  protected resetForm() {
    this.changePasswordRequest.set(initialState);
    this.changePasswordForm().reset();
  }

  protected ariaInvalidState(field: FieldTree<unknown>): boolean | undefined {
    return field().touched() && !field().pending()
      ? field().errors().length > 0
      : undefined;
  }
}
