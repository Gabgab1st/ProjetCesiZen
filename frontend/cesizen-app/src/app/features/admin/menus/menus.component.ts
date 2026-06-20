import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';

import { HttpClient } from '@angular/common/http';
import { Menu } from '../../../shared/models/menu.model';

@Component({
  selector: 'app-admin-menus',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTooltipModule,
  ],
  templateUrl: './menus.component.html',
  styleUrls: ['./menus.component.scss'],
})
export class AdminMenusComponent implements OnInit {
  private apiUrl = 'http://localhost:5030/api/menus';

  menus: Menu[] = [];
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  editingMenu: Menu | null = null;
  showForm = false;

  menuForm!: FormGroup;
  displayedColumns = ['ordre', 'libelle', 'pages', 'actions'];

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadMenus();
  }

  private initForm(menu?: Menu): void {
    this.menuForm = this.fb.group({
      libelle: [menu?.libelle ?? '', [Validators.required, Validators.minLength(2)]],
      ordre: [menu?.ordre ?? 1, [Validators.required, Validators.min(1)]],
    });
  }

  private loadMenus(): void {
    this.loading = true;
    this.http.get<Menu[]>(this.apiUrl).subscribe({
      next: (menus) => {
        this.menus = menus;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erreur lors du chargement des menus.';
        this.loading = false;
      }
    });
  }

  openCreate(): void {
    this.editingMenu = null;
    this.initForm();
    this.showForm = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  openEdit(menu: Menu): void {
    this.editingMenu = menu;
    this.initForm(menu);
    this.showForm = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelForm(): void {
    this.showForm = false;
    this.editingMenu = null;
    this.errorMessage = '';
  }

  onSubmit(): void {
    if (this.menuForm.invalid) {
      this.menuForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    const dto = this.menuForm.value;

    if (this.editingMenu) {
      this.http.put<Menu>(`${this.apiUrl}/${this.editingMenu.menuId}`, dto).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Menu modifié avec succès.';
          this.showForm = false;
          this.loadMenus();
        },
        error: () => {
          this.saving = false;
          this.errorMessage = 'Erreur lors de la modification.';
        }
      });
    } else {
      this.http.post<Menu>(this.apiUrl, dto).subscribe({
        next: () => {
          this.saving = false;
          this.successMessage = 'Menu créé avec succès.';
          this.showForm = false;
          this.loadMenus();
        },
        error: () => {
          this.saving = false;
          this.errorMessage = 'Erreur lors de la création.';
        }
      });
    }
  }

  deleteMenu(menu: Menu): void {
    if (!confirm(`Supprimer le menu "${menu.libelle}" ? Les pages associées seront orphelines.`)) return;

    this.http.delete(`${this.apiUrl}/${menu.menuId}`).subscribe({
      next: () => {
        this.successMessage = 'Menu supprimé.';
        this.loadMenus();
      },
      error: () => {
        this.errorMessage = 'Erreur lors de la suppression.';
      }
    });
  }

  get libelleControl() { return this.menuForm.get('libelle'); }
  get ordreControl() { return this.menuForm.get('ordre'); }
}