# ais-client

The web client of the Accountability Information System: an Angular 22 standalone, zoneless,
signals-first single-page app (signal forms, `httpResource`/`rxResource`) styled with Tailwind 4.
It talks to the .NET API in `../ais-server` and is deployed to Azure Static Web Apps.

## Prerequisites

- Node.js 22 and npm.
- The API running locally on `https://localhost:4001/` (see `src/environments/environment.ts`).
  The API uses the ASP.NET Core dev certificate; trust it with `dotnet dev-certs https --trust`.
  If you export it for the dev server, keep `localhost.pem` / `localhost.key` out of git
  (they are in `.gitignore`).

## Scripts

| Command | What it does |
| --- | --- |
| `npm start` | Dev server on `http://localhost:4200/` with live reload. |
| `npm test` | Unit tests (Vitest) in watch mode. |
| `npm run test:ci` | Unit tests once, as CI runs them. |
| `npm run build` | Production build into `dist/ais-client/browser`. |

Plain `ng serve`, `ng build` and `ng test` work too. `src/environments/build-info.ts` is a
committed placeholder; CI overwrites it with the real build date before building
(`scripts/generate-build-info.mjs`, which only writes when `CI` or `BUILD_INFO` is set).
Do not commit a generated version of that file.

## Environments

- `environment.ts`: local development, API on `https://localhost:4001/`.
- `environment.prod.ts`: production, swapped in by the `production` build configuration.

## How authentication works

The API keeps the session in HttpOnly cookies (access and refresh token) and issues an
`XSRF-TOKEN` cookie for antiforgery. Three interceptors in `src/app/app.config.ts` handle it,
outermost first:

1. `httpErrorResponseInterceptor` records problem details for the page and shows a toast,
   unless the request is marked `inlineErrors()` (forms show their errors above the form).
2. `authInterceptor` refreshes the session once on a 401 and retries. Parallel 401s share one
   refresh, because the server rotates the refresh token. Requests marked `anonymousRequest()`
   (login, 2FA, refresh, logout, ...) are never refreshed.
3. `csrfInterceptor` adds `withCredentials` and the `X-XSRF-TOKEN` header. It is innermost so
   a retry picks up the token issued by the refresh.

The signed-in user is restored before the first render (`provideAppInitializer` calling
`AuthService.checkAuth()`). `authGuard` protects signed-in pages and `adminGuard` the
`/admin` routes. Data resources that belong to a user key on `AuthService.userId()`, so they
empty on logout.

## Project layout

```
src/app
  auth/       login, registration, 2FA, password pages
  core/       app-wide singletons: guards, http (interceptors, endpoints, error service),
              auth service, app routes
  features/   one folder per area (family/warranty-records, family/warranty-brands, admin, ...)
  layout/     header, footer, not-found
  shared/     reusable UI and helpers (pagination, toast, confirm dialog, form errors, validators)
```

API paths live in `src/app/core/http/endpoints-config.ts`; build URLs with `apiUrl(...)`.

## Deployment

`.github/workflows/build-and-deploy.yml` runs `npm audit`, the unit tests and a build on every
pull request (job "Frontend: Audit, Test and Build"). On `main` it then builds and deploys
`dist/ais-client/browser` with the Static Web Apps CLI. Response headers (CSP, HSTS, frame
protection) are set in `src/staticwebapp.config.json`; the CSP is report-only for now.
