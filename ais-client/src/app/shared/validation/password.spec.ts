import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection, signal } from '@angular/core';
import { form } from '@angular/forms/signals';
import { passwordPolicy } from './password';

function errorsFor(password: string): string[] {
  TestBed.configureTestingModule({ providers: [provideZonelessChangeDetection()] });
  return TestBed.runInInjectionContext(() => {
    const f = form(signal({ password }), p => passwordPolicy(p.password));
    return f.password().errors().map(e => e.message ?? e.kind);
  });
}

describe('passwordPolicy', () => {
  it('accepts a password that meets the server policy', () => {
    expect(errorsFor('Str0ng!pass')).toEqual([]);
  });

  it('rejects short passwords', () => {
    expect(errorsFor('Aa1!')).toContain('Password must be at least 8 characters.');
  });

  it('lists the missing character classes', () => {
    expect(errorsFor('alllowercase')).toEqual([
      'Password must contain an uppercase letter, a digit, a symbol.',
    ]);
  });
});
