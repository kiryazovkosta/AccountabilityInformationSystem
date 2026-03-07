import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';

const UNSAFE_HTTP_METHODS = new Set(['POST', 'PUT', 'PATCH', 'DELETE']);
const CSRF_COOKIE_NAME = 'XSRF-TOKEN';
const CSRF_HEADER_NAME = 'X-XSRF-TOKEN';

export const csrfInterceptor: HttpInterceptorFn = (request: HttpRequest<unknown>, next: HttpHandlerFn) => {
  let modifiedRequest = request.clone({ withCredentials: true });

  if (!UNSAFE_HTTP_METHODS.has(request.method.toUpperCase())) {
    return next(modifiedRequest);
  }

  const csrfToken = readCookie(CSRF_COOKIE_NAME);
  if (!csrfToken) {
    return next(modifiedRequest);
  }

  modifiedRequest = modifiedRequest.clone({
    setHeaders: {
      [CSRF_HEADER_NAME]: csrfToken
    }
  });

  return next(modifiedRequest);
};

function readCookie(name: string): string | null {
  const cookiePrefix = `${name}=`;
  const cookies = document.cookie.split(';');

  for (const cookie of cookies) {
    const normalizedCookie = cookie.trim();
    if (normalizedCookie.startsWith(cookiePrefix)) {
      return decodeURIComponent(normalizedCookie.substring(cookiePrefix.length));
    }
  }

  return null;
}
