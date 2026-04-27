import { Component, inject, signal } from '@angular/core';
import { ResendEmailConfirmationRequest } from './resend-email-confirmation-request';
import { email, form, FormField, FormRoot, required, schema } from '@angular/forms/signals';
import { FormError } from "../../shared/form-error/form-error";
import { AuthService } from '../../services/shared/auth.service';

const initialState: ResendEmailConfirmationRequest = {
  email: ''
}

export const resendSchema = schema<ResendEmailConfirmationRequest>((path) => {
  required(path.email, { message: 'Email is required.' });
  email(path.email, { message: 'Email is not valid.' });
});

@Component({
  selector: 'app-resend-email-confirmation',
  imports: [FormField, FormRoot, FormError],
  templateUrl: './resend-email-confirmation.html',
  styleUrl: './resend-email-confirmation.css',
})
export class ResendEmailConfirmation {

  authService: AuthService = inject(AuthService);
  protected readonly resendRequest = signal<ResendEmailConfirmationRequest>(initialState);

  protected readonly resendForm = form(
    this.resendRequest,
    resendSchema,
    {
      submission: {
        action: async (form) => {
          console.log("resending...");
          this.authService.resendEmail(this.resendForm().value())
        }
      },
    });
}
