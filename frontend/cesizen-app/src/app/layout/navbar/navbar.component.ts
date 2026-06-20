// src/app/layout/navbar/navbar.component.ts
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
  ],
  template: `
    <mat-toolbar color="primary">
      <span class="logo" routerLink="/">CESIZen</span>
<span class="spacer"></span>
<button mat-button routerLink="/">
  <mat-icon>home</mat-icon>
  Accueil
</button>
<button mat-button routerLink="/exercices">
  <mat-icon>self_improvement</mat-icon>
  Exercices
</button>
      
      <ng-container *ngIf="isLoggedIn; else loginBtn">
        <button mat-button [matMenuTriggerFor]="userMenu">
          <mat-icon>account_circle</mat-icon>
          {{ currentUser?.prenom }}
        </button>
        <mat-menu #userMenu="matMenu">
          <button mat-menu-item *ngIf="isAdmin" routerLink="/admin">
            <mat-icon>admin_panel_settings</mat-icon>
            Administration
          </button>
          <button mat-menu-item *ngIf="isAdmin" routerLink="/admin/users">
          <mat-icon>group</mat-icon>
            Utilisateurs
          </button>
          <button mat-menu-item *ngIf="isAdmin" routerLink="/admin/menus">
  <mat-icon>menu_book</mat-icon>
  Menus
</button>
          <button mat-menu-item routerLink="/auth/reset-password">
            <mat-icon>lock_reset</mat-icon>
            Changer mon mot de passe
          </button>
          <button mat-menu-item (click)="logout()">
            <mat-icon>logout</mat-icon>
            Déconnexion
          </button>
        </mat-menu>
      </ng-container>
      <ng-template #loginBtn>
        <button mat-button routerLink="/auth/login">
          <mat-icon>login</mat-icon>
          Connexion
        </button>
        <button mat-raised-button routerLink="/auth/register">
          S'inscrire
        </button>
      </ng-template>
    </mat-toolbar>
  `,
  styles: [`
    mat-toolbar {
      position: sticky;
      top: 0;
      z-index: 100;
    }
    .logo {
      font-size: 1.4rem;
      font-weight: bold;
      cursor: pointer;
      letter-spacing: 1px;
    }
    .spacer {
      flex: 1 1 auto;
    }
  `]
})
export class NavbarComponent implements OnInit {
  isLoggedIn = false;
  isAdmin = false;
  currentUser: any = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.isLoggedIn = !!user;
      this.isAdmin = this.authService.isAdmin();
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}