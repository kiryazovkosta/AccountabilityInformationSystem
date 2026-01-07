import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', loadComponent: () => import('./features/home/home').then(m => m.Home) },
    { path: 'home', loadComponent: () => import('./features/home/home').then(m => m.Home) },
    { path: 'features', loadComponent: () => import('./features/features/features').then(m => m.Features) },
    { path: 'pricing', loadComponent: () => import('./features/pricing/pricing').then(m => m.Pricing) },
    { path: 'contact', loadComponent: () => import('./features/contact/contact').then(m => m.Contact) },
    { path: 'kendo-demos', loadComponent: () => import('./features/kendo-demos/kendo-demos').then(m => m.KendoDemos) },
    { 
        path: 'auth',
        children: [
            { path: 'login', loadComponent: () => import('./auth/login/login').then(l => l.Login) },
            { path: 'user-profile', loadComponent: () => import('./auth/user-profile/user-profile').then(l => l.UserProfile) },
            { path: 'logout', loadComponent: () => import('./auth/logout/logout').then(l => l.Logout) }
        ]
     }
];
