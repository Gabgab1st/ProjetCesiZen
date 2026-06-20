// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home.component').then(
        (m) => m.HomeComponent
      ),
  },
  {
    path: 'auth',
    children: [
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full',
      },
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login.component').then(
            (m) => m.LoginComponent
          ),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register.component').then(
            (m) => m.RegisterComponent
          ),
      },
      {
        path: 'reset-password',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./features/auth/reset-password/reset-password.component').then(
            (m) => m.ResetPasswordComponent
          ),
      },
    ],
  },
  {
    path: 'info',
    redirectTo: 'info/accueil',
    pathMatch: 'full',
  },
  {
    path: 'info/:slug',
    loadComponent: () =>
      import('./features/info/page-detail/page-detail.component').then(
        (m) => m.PageDetailComponent
      ),
  },
  {
    path: 'exercices',
    loadComponent: () =>
      import('./features/exercices/exercice-detail/exercice-detail.component').then(
        (m) => m.ExerciceDetailComponent
      ),
  },
  {
    path: 'admin',
    canActivate: [authGuard],
    children: [
      {
        path: '',
        redirectTo: 'pages',
        pathMatch: 'full',
      },
      {
        path: 'pages',
        loadComponent: () =>
          import('./features/admin/pages/pages.component').then(
            (m) => m.AdminPagesComponent
          ),
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];