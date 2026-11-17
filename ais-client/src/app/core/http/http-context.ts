import { HttpContext, HttpContextToken } from '@angular/common/http';

/**
 * Set on requests whose 401 must reach the caller as-is (login, 2FA, refresh, logout, ...).
 * authInterceptor will not try to refresh the session for them.
 */
export const SKIP_REFRESH = new HttpContextToken<boolean>(() => false);

/**
 * Set on form submissions that show their errors inline. httpErrorResponseInterceptor still
 * records the problem details, but does not show a toast for them.
 */
export const SKIP_ERROR_TOAST = new HttpContextToken<boolean>(() => false);

/** Context for anonymous auth endpoints: no refresh on 401. */
export function anonymousRequest(context = new HttpContext()): HttpContext {
  return context.set(SKIP_REFRESH, true);
}

/**
 * Set on requests whose failure the caller handles completely (e.g. the session refresh).
 * httpErrorResponseInterceptor neither records nor toasts their errors.
 */
export const SILENT_ERRORS = new HttpContextToken<boolean>(() => false);

/** Context for form submissions that render their own errors. */
export function inlineErrors(context = new HttpContext()): HttpContext {
  return context.set(SKIP_ERROR_TOAST, true);
}

/** Context for requests whose errors the caller handles itself: nothing is recorded or shown. */
export function silentRequest(context = new HttpContext()): HttpContext {
  return context.set(SILENT_ERRORS, true);
}
