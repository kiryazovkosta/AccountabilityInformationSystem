import { Component, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/shared/auth.service';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-logout',
  imports: [],
  templateUrl: './logout.html',
  styleUrl: './logout.css'
})
export class Logout {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly logoutResult = toSignal(this.authService.logout(), { initialValue: null });

  constructor() {
    effect(() => {
      if (this.logoutResult() === true) {
        this.router.navigate(['/home']);
      }
    });
  }
}
