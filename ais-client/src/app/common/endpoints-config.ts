export const Endpoints = {
  login: 'api/identity/auth/login',
  register: 'api/identity/auth/register',
  confirmEmail: 'api/identity/auth/confirm-email',
  logout: 'api/identity/auth/logout',
  checkAuth: 'api/identity/users/me',
  refresh: 'api/identity/auth/refresh',
} as const;
