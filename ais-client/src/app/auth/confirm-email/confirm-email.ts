import { rxResource } from '@angular/core/rxjs-interop';
import { AuthService } from './../../services/shared/auth.service';
import { Component, inject, input } from '@angular/core';

@Component({
  selector: 'app-confirm-email',
  imports: [],
  templateUrl: './confirm-email.html',
  styleUrl: './confirm-email.css',
})
export class ConfirmEmail {
  private readonly authService: AuthService = inject(AuthService);

  userId = input.required<string>();
  code = input.required<string>();

  confirmResource = rxResource({
    params: () => ({ userId: this.userId(), code: this.code() }),
    stream: ({params}) => this.authService.confirmEmail(params.userId, params.code),
  });
}
