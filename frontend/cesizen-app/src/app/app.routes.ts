// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'info',
    pathMatch: 'full'
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login.component').then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register.component').then(m => m.RegisterComponent)
      }
    ]
  },
  {
    path: 'info',
    loadComponent: () =>
      import('./features/info/page-detail/page-detail.component').then(m => m.PageDetailComponent)
  },
  {
    path: 'info/:slug',
    loadComponent: () =>
      import('./features/info/page-detail/page-detail.component').then(m => m.PageDetailComponent)
  },
  {
    path: 'exercices',
    loadComponent: () =>
      import('./features/exercices/exercice-detail/exercice-detail.component').then(m => m.ExerciceDetailComponent)
  },
  {
    path: '**',
    redirectTo: 'info'
  }
];