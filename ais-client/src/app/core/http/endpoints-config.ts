import { environment } from '../../../environments/environment';

// All API paths, relative to environment.apiBaseUrl.
// csrfInterceptor sets withCredentials on every request, so services do not need to.
export const Endpoints = {
  login: 'api/identity/auth/login',
  register: 'api/identity/auth/register',
  confirmEmail: 'api/identity/auth/confirm-email',
  logout: 'api/identity/auth/logout',
  checkAuth: 'api/identity/users/me',
  refresh: 'api/identity/auth/refresh',
  setup2fa: 'api/identity/auth/2fa/setup',
  verify2fa: 'api/identity/auth/2fa/verify',
  resendConfirmationEmail: 'api/identity/auth/resend-email-confirmation',
  forgotPassword: 'api/identity/auth/forgot-password',
  resetPassword: 'api/identity/auth/reset-password',
  changePassword: 'api/identity/auth/change-password',
  newDevice2fa: 'api/identity/auth/2fa/new-device',

  adminUsers: 'api/admin/users',
  warrantyRecords: 'api/family/warranty-records',
  warrantyBrands: 'api/family/warranty-brands',
  ikunks: 'api/flow/ikunks',
} as const;

export function apiUrl(path: string, ...segments: string[]): string {
  const suffix = segments.map(encodeURIComponent).join('/');
  return `${environment.apiBaseUrl}${path}${suffix ? `/${suffix}` : ''}`;
}
