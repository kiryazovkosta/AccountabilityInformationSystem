import { minLength, required, SchemaPath, validate } from '@angular/forms/signals';

/**
 * Mirrors the server's Identity PasswordOptions
 * (ais-server WebApplicationBuilderExtensions.AddAuthenticationServices):
 * at least 8 characters, with a digit, a lowercase letter, an uppercase letter and a symbol.
 */
export const PASSWORD_MIN_LENGTH = 8;
export const PASSWORD_HINT =
  `At least ${PASSWORD_MIN_LENGTH} characters, with an uppercase letter, a lowercase letter, a digit and a symbol.`;

export function passwordPolicy(path: SchemaPath<string>, label = 'Password'): void {
  required(path, { message: `${label} is required.` });
  minLength(path, PASSWORD_MIN_LENGTH, {
    message: `${label} must be at least ${PASSWORD_MIN_LENGTH} characters.`,
  });
  validate(path, ({ value }) => {
    const password = value() ?? '';
    if (!password) return null;
    const missing: string[] = [];
    if (!/[A-Z]/.test(password)) missing.push('an uppercase letter');
    if (!/[a-z]/.test(password)) missing.push('a lowercase letter');
    if (!/[0-9]/.test(password)) missing.push('a digit');
    if (!/[^a-zA-Z0-9]/.test(password)) missing.push('a symbol');
    return missing.length
      ? { kind: 'passwordPolicy', message: `${label} must contain ${missing.join(', ')}.` }
      : null;
  });
}
