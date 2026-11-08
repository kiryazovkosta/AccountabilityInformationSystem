import {
  ApplicationConfig,
  inject,
  provideAppInitializer,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import { routes } from './app.routes';
import { csrfInterceptor } from './core/http/csrf.interceptor';
import { authInterceptor } from './core/http/auth.interceptor';
import { httpErrorResponseInterceptor } from './core/http/http-error-response.interceptor';
import { AuthService } from './core/auth/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(
      routes,
      withComponentInputBinding()
    ),
    provideHttpClient(
      // Order matters (outermost first):
      // - httpErrorResponseInterceptor sees the final outcome after any refresh-and-retry;
      // - authInterceptor refreshes the session on 401 and retries;
      // - csrfInterceptor is innermost so every attempt, including the retry, reads the
      //   current XSRF-TOKEN cookie (the server issues a new one on refresh).
      withInterceptors(
        [
          httpErrorResponseInterceptor,
          authInterceptor,
          csrfInterceptor,
        ]
      )
    ),
    // Restore the signed-in user before the first render, so the header is right after a reload.
    provideAppInitializer(() => firstValueFrom(inject(AuthService).checkAuth())),
  ]
};
