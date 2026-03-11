import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, Router } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { routes } from './app.routes';
import { csrfInterceptor } from './core/http/csrf.interceptor';
import { authInterceptor } from './services/shared/auth.interceptor';
import { AuthService } from './services/shared/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([csrfInterceptor, authInterceptor])),
    provideAppInitializer(async () => {
      const authService = inject(AuthService);
      const router = inject(Router);
      const loggedIn = await firstValueFrom(authService.checkAuth());
      if (!loggedIn) {
        await router.navigate(['/auth/login']);
      }
    })
  ]
};
