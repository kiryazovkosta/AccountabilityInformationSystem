import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../../services/shared/auth.service';
import { firstValueFrom } from 'rxjs';

export const authGuard: CanActivateFn = async () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const loggedIn = await firstValueFrom(authService.checkAuth());
  if (!loggedIn) {
    router.navigate(['/auth/login']);
    return false;
  }
  return true;
};
