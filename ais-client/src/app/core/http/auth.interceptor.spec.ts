import { TestBed } from '@angular/core/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter, Router } from '@angular/router';
import { provideZonelessChangeDetection } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { authInterceptor } from './auth.interceptor';
import { httpErrorResponseInterceptor } from './http-error-response.interceptor';
import { csrfInterceptor } from './csrf.interceptor';
import { AuthService } from '../auth/auth.service';
import { HttpErrorService } from './http-error.service';
import { environment } from '../../../environments/environment';
import { Endpoints } from './endpoints-config';

const api = (path: string) => `${environment.apiBaseUrl}${path}`;

function setXsrfCookie(value: string): void {
  document.cookie = `XSRF-TOKEN=${value}; path=/`;
}

describe('authInterceptor', () => {
  let http: HttpClient;
  let backend: HttpTestingController;
  let auth: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        provideHttpClient(
          withInterceptors([httpErrorResponseInterceptor, authInterceptor, csrfInterceptor])
        ),
        provideHttpClientTesting(),
      ],
    });
    http = TestBed.inject(HttpClient);
    backend = TestBed.inject(HttpTestingController);
    auth = TestBed.inject(AuthService);
    vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
  });

  afterEach(() => {
    backend.verify();
    setXsrfCookie('');
  });

  it('shares one refresh between parallel 401s and retries each request once', async () => {
    const results = Promise.all([
      firstValueFrom(http.get<string>(api('api/a'))),
      firstValueFrom(http.get<string>(api('api/b'))),
      firstValueFrom(http.get<string>(api('api/c'))),
    ]);

    for (const path of ['api/a', 'api/b', 'api/c']) {
      backend.expectOne(api(path)).flush(null, { status: 401, statusText: 'Unauthorized' });
    }

    const refreshes = backend.match(api(Endpoints.refresh));
    expect(refreshes.length).toBe(1);
    refreshes[0].flush(null, { status: 200, statusText: 'OK' });

    backend.expectOne(api('api/a')).flush('A');
    backend.expectOne(api('api/b')).flush('B');
    backend.expectOne(api('api/c')).flush('C');

    expect(await results).toEqual(['A', 'B', 'C']);
    expect(auth.isLoggedIn()).toBe(true);
  });

  it('sends the XSRF token issued by the refresh on the retried request', async () => {
    setXsrfCookie('old-token');
    const result = firstValueFrom(http.post<string>(api('api/items'), {}));

    const first = backend.expectOne(api('api/items'));
    expect(first.request.headers.get('X-XSRF-TOKEN')).toBe('old-token');
    first.flush(null, { status: 401, statusText: 'Unauthorized' });

    setXsrfCookie('new-token'); // the server rotates the cookie on refresh
    backend.expectOne(api(Endpoints.refresh)).flush(null, { status: 200, statusText: 'OK' });

    const retry = backend.expectOne(api('api/items'));
    expect(retry.request.headers.get('X-XSRF-TOKEN')).toBe('new-token');
    retry.flush('ok');

    expect(await result).toBe('ok');
  });

  it('fails the request and clears the session when the refresh fails', async () => {
    const result = firstValueFrom(http.get(api('api/a'))).catch(e => e);

    backend.expectOne(api('api/a')).flush(null, { status: 401, statusText: 'Unauthorized' });
    backend.expectOne(api(Endpoints.refresh)).flush(null, { status: 401, statusText: 'Unauthorized' });

    const error = await result;
    expect(error.status).toBe(401);
    expect(auth.isLoggedIn()).toBe(false);
  });

  it('does not refresh on a failed login and records the error for the page', async () => {
    const result = firstValueFrom(
      auth.login({ username: 'u', password: 'wrong', remember: false, code: '' })
    ).catch(e => e);

    backend.expectOne(api(Endpoints.login)).flush(
      { status: 401, detail: 'Invalid username or password.' },
      { status: 401, statusText: 'Unauthorized' }
    );
    backend.expectNone(api(Endpoints.refresh));

    expect((await result).status).toBe(401);
    expect(TestBed.inject(HttpErrorService).errorDetail()).toBe('Invalid username or password.');
  });
});
