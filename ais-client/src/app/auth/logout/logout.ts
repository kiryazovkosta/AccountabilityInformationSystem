import { ChangeDetectionStrategy, Component, effect, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/shared/auth.service';
import { toSignal } from '@angular/core/rxjs-interop';
import { APP_ROUTES } from '../../common/app-routes';

@Component({
  selector: 'app-logout',
  imports: [],
  templateUrl: './logout.html',
  styleUrl: './logout.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Logout {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  private readonly logoutResult = toSignal(this.authService.logout(), { initialValue: null });

  constructor() {
    effect(() => {
      if (this.logoutResult() === true) {
        this.router.navigate([APP_ROUTES.HOME]);
      }
    });
  }
}
