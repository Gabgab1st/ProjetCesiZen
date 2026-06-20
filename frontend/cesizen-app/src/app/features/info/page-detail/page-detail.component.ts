import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';

import { PageService } from '../../../core/services/page.service';
import { Page } from '../../../shared/models/page.model';

@Component({
  selector: 'app-page-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
  ],
  templateUrl: './page-detail.component.html',
  styleUrls: ['./page-detail.component.scss'],
})
export class PageDetailComponent implements OnInit {
  page: Page | null = null;
  loading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private pageService: PageService
  ) {}

  ngOnInit(): void {
    // Recharge le composant si le slug change (navigation entre pages)
    this.route.paramMap.pipe(
      switchMap(params => {
        const slug = params.get('slug') ?? '';
        this.loading = true;
        this.errorMessage = '';
        this.page = null;
        return this.pageService.getBySlug(slug);
      })
    ).subscribe({
      next: (page) => {
        this.page = page;
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        if (err.status === 404) {
          this.errorMessage = 'Cette page est introuvable.';
        } else {
          this.errorMessage = 'Une erreur est survenue lors du chargement de la page.';
        }
      },
    });
  }
}