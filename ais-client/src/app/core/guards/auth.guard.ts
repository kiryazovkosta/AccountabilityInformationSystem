import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { firstValueFrom } from 'rxjs';
import { APP_ROUTES } from '../app-routes';

export const authGuard: CanActivateFn = async (_route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  if (authService.isLoggedIn()) return true;
  const loggedIn = await firstValueFrom(authService.checkAuth());
  return loggedIn
    ? true
    : router.createUrlTree([APP_ROUTES.LOGIN], { queryParams: { returnUrl: state.url } });
};
