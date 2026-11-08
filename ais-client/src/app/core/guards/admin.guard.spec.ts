import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection, signal } from '@angular/core';
import { provideRouter, Route, UrlSegment, UrlTree } from '@angular/router';
import { of } from 'rxjs';
import { adminGuard } from './admin.guard';
import { AuthService } from '../auth/auth.service';

function setup(state: { loggedIn: boolean; admin: boolean }) {
  const authStub = {
    isLoggedIn: signal(state.loggedIn),
    isAdministrator: signal(state.admin),
    checkAuth: () => of(state.loggedIn),
  };
  TestBed.configureTestingModule({
    providers: [
      provideZonelessChangeDetection(),
      provideRouter([]),
      { provide: AuthService, useValue: authStub },
    ],
  });
  return TestBed.runInInjectionContext(() => adminGuard({} as Route, [] as UrlSegment[], {} as never));
}

describe('adminGuard', () => {
  it('lets administrators in', async () => {
    expect(await setup({ loggedIn: true, admin: true })).toBe(true);
  });

  it('sends other signed-in users to Home', async () => {
    const result = (await setup({ loggedIn: true, admin: false })) as UrlTree;
    expect(result.toString()).toBe('/home');
  });

  it('sends anonymous visitors to login', async () => {
    const result = (await setup({ loggedIn: false, admin: false })) as UrlTree;
    expect(result.toString()).toBe('/auth/login');
  });
});
