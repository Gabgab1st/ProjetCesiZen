import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MenuService } from '../../core/services/menu.service';
import { AuthService } from '../../core/services/auth.service';
import { Menu } from '../../shared/models/menu.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  menus: Menu[] = [];
  loading = true;
  errorMessage = '';

  constructor(
    private menuService: MenuService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.menuService.getAll().subscribe({
      next: (menus) => {
        const isLoggedIn = this.authService.isLoggedIn();
        this.menus = menus
          .map(m => ({
            ...m,
            // Si non connecté, ne garder que les pages publiques
            pages: m.pages?.filter(p => isLoggedIn || p.isPublic) ?? []
          }))
          // Ne garder que les menus qui ont au moins une page visible
          .filter(m => m.pages.length > 0);
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Impossible de charger le contenu.';
        this.loading = false;
      }
    });
  }

  navigateTo(slug: string): void {
    this.router.navigate(['/info', slug]);
  }

  navigateToExercices(): void {
    this.router.navigate(['/exercices']);
  }
}