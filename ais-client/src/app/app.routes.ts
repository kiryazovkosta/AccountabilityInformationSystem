import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    { path: '', loadComponent: () => import('./features/home/home').then(m => m.Home) },
    { path: 'home', loadComponent: () => import('./features/home/home').then(m => m.Home) },
    { path: 'ikunks', loadComponent: () => import('./features/measurement-flow/ikunks/ikunks-list/ikunks-list').then(m => m.IkunksList), canActivate: [authGuard] },
    { path: 'pricing', loadComponent: () => import('./features/pricing/pricing').then(m => m.Pricing) },
    { path: 'contact', loadComponent: () => import('./features/contact/contact').then(m => m.Contact) },
    {
        path: 'auth',
        children: [
            { path: 'register', loadComponent: () => import('./auth/register-user/register-user').then(r => r.RegisterUser) },
            { path: 'confirm-email', loadComponent: () => import('./auth/confirm-email/confirm-email').then(ce => ce.ConfirmEmail) },
            { path: 'resend-email-confirmation', loadComponent: () => import('./auth/resend-email-confirmation/resend-email-confirmation').then(re => re.ResendEmailConfirmation) },
            { path: 'setup-2fa', loadComponent: () => import('./auth/setup-two-factor/setup-two-factor').then(m => m.SetupTwoFactor) },
            { path: 'login', loadComponent: () => import('./auth/login/login').then(l => l.Login) },
            { path: 'user-profile', loadComponent: () => import('./auth/user-profile/user-profile').then(l => l.UserProfile), canActivate: [authGuard] },
            { path: 'logout', loadComponent: () => import('./auth/logout/logout').then(l => l.Logout) }
        ]
    }
];
