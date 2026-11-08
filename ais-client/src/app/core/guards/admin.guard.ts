import { inject } from '@angular/core';
import { CanMatchFn, Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { ToastService } from '../../shared/toast/toast-service';
import { APP_ROUTES } from '../app-routes';

/** Lets only administrators match /admin routes. The server still enforces the role. */
export const adminGuard: CanMatchFn = async () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const toastService = inject(ToastService);

  if (!authService.isLoggedIn()) {
    const loggedIn = await firstValueFrom(authService.checkAuth());
    if (!loggedIn) {
      return router.createUrlTree([APP_ROUTES.LOGIN]);
    }
  }

  if (authService.isAdministrator()) {
    return true;
  }

  toastService.show('You do not have permission to open that page.', 'warning');
  return router.createUrlTree([APP_ROUTES.HOME]);
};
