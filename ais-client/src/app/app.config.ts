import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZonelessChangeDetection } from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { csrfInterceptor } from './core/http/csrf.interceptor';
import { authInterceptor } from './services/shared/auth.interceptor';
import { httpErrorResponseInterceptor } from './services/shared/http-error-response.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(
      routes,
      withComponentInputBinding()
    ),
    provideHttpClient(
      withInterceptors(
        [
          csrfInterceptor, 
          authInterceptor,
          httpErrorResponseInterceptor
        ]
      )
    ),
  ]
};
