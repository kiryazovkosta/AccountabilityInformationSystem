import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../services/shared/auth.service';
import { firstValueFrom } from 'rxjs';
import { APP_ROUTES } from '../../common/app-routes';

export const authGuard: CanActivateFn = async () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (authService.isLoggedIn()) return true;
  const loggedIn = await firstValueFrom(authService.checkAuth());
  if (!loggedIn) {
    router.navigate([APP_ROUTES.LOGIN]);
    return false;
  }
  return true;
};
