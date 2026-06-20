import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PageService } from '../../../core/services/page.service';
import { MenuService } from '../../../core/services/menu.service';
import { Page } from '../../../shared/models/page.model';
import { Menu } from '../../../shared/models/menu.model';

@Component({
  selector: 'app-admin-pages',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTooltipModule
  ],
  templateUrl: './pages.component.html',
  styleUrls: ['./pages.component.scss'],
})
export class AdminPagesComponent implements OnInit {
  pages: Page[] = [];
  menus: Menu[] = [];
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  editingPage: Page | null = null;
  showForm = false;

  pageForm!: FormGroup;
  displayedColumns = ['titre', 'slug', 'menu', 'isPublic', 'actions'];

  constructor(
    private fb: FormBuilder,
    private pageService: PageService,
    private menuService: MenuService
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadData();
  }

  private initForm(page?: Page): void {
    this.pageForm = this.fb.group({
      titre: [page?.titre ?? '', [Validators.required, Validators.minLength(3)]],
      slug: [page?.slug ?? '', [Validators.required, Validators.pattern(/^[a-z0-9-]+$/)]],
      contenu: [page?.contenu ?? '', Validators.required],
      menuId: [page?.menuId ?? '', Validators.required],
      isPublic: [page?.isPublic ?? true],
    });
  }

  private loadData(): void {
    this.loading = true;
    this.menuService.getAll().subscribe(menus => {
      this.menus = menus;
      this.pageService.getAll().subscribe({
        next: (pages) => {
          this.pages = pages;
          this.loading = false;
        },
        error: () => {
          this.errorMessage = 'Erreur lors du chargement des pages.';
          this.loading = false;
        }
      });
    });
  }

  getMenuLibelle(menuId: number): string {
    return this.menus.find(m => m.menuId === menuId)?.libelle ?? '-';
  }

  openCreate(): void {
    this.editingPage = null;
    this.initForm();
    this.showForm = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  openEdit(page: Page): void {
    this.editingPage = page;
    this.initForm(page);
    this.showForm = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingPage = null;
    this.errorMessage = '';
  }

  onSubmit(): void {
    if (this.pageForm.invalid) {
      this.pageForm.markAllAsTouched();
      return;
    }
 console.log('DTO envoyé:', this.pageForm.value);
    this.saving = true;
    this.errorMessage = '';
    const dto = this.pageForm.value;

    if (this.editingPage) {
      this.pageService.update(this.editingPage.pageId, dto).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Page modifiée avec succès.';
          this.showForm = false;
          this.loadData();
        },
        error: () => {
          this.saving = false;
          this.errorMessage = 'Erreur lors de la modification.';
        }
      });
    } else {
      this.pageService.create(dto).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Page créée avec succès.';
          this.showForm = false;
          this.loadData();
        },
        error: () => {
          this.saving = false;
          this.errorMessage = 'Erreur lors de la création.';
        }
      });
    }
  }

  deletePage(page: Page): void {
    if (!confirm(`Supprimer la page "${page.titre}" ?`)) return;
    this.pageService.delete(page.pageId).subscribe({
      next: () => {
        this.successMessage = 'Page supprimée.';
        this.loadData();
      },
      error: () => {
        this.errorMessage = 'Erreur lors de la suppression.';
      }
    });
  }

  get titreControl() { return this.pageForm.get('titre'); }
  get slugControl() { return this.pageForm.get('slug'); }
  get contenuControl() { return this.pageForm.get('contenu'); }
  get menuIdControl() { return this.pageForm.get('menuId'); }
}