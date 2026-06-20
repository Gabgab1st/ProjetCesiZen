import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';

import { HttpClient } from '@angular/common/http';

interface UserDto {
  utilisateurId: number;
  nom: string;
  prenom: string;
  email: string;
  actif: boolean;
  role: string;
  createdAt: Date;
}

@Component({
  selector: 'app-admin-users',
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
    MatCardModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTooltipModule,
    MatChipsModule,
  ],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
})
export class AdminUsersComponent implements OnInit {
  private apiUrl = 'http://localhost:5030/api/users';

  users: UserDto[] = [];
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  showForm = false;

  userForm!: FormGroup;
  displayedColumns = ['nom', 'email', 'role', 'actif', 'createdAt', 'actions'];

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.initForm();
    this.loadUsers();
  }

  private initForm(): void {
    this.userForm = this.fb.group({
      nom: ['', [Validators.required, Validators.minLength(2)]],
      prenom: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      motDePasse: ['', [Validators.required, Validators.minLength(8)]],
      roleId: [2, Validators.required],
    });
  }

  private loadUsers(): void {
    this.loading = true;
    this.http.get<UserDto[]>(this.apiUrl).subscribe({
      next: (users) => {
        this.users = users;
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Erreur lors du chargement des utilisateurs.';
        this.loading = false;
      }
    });
  }

  openCreate(): void {
    this.initForm();
    this.showForm = true;
    this.errorMessage = '';
    this.successMessage = '';
  }

  cancelForm(): void {
    this.showForm = false;
    this.errorMessage = '';
  }

  onSubmit(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';

    this.http.post<UserDto>(this.apiUrl, this.userForm.value).subscribe({
      next: () => {
        this.saving = false;
        this.successMessage = 'Utilisateur créé avec succès.';
        this.showForm = false;
        this.loadUsers();
      },
      error: (err) => {
        this.saving = false;
        if (err.status === 409) {
          this.errorMessage = 'Cette adresse e-mail est déjà utilisée.';
        } else {
          this.errorMessage = 'Erreur lors de la création.';
        }
      }
    });
  }

  toggleActive(user: UserDto): void {
    const action = user.actif ? 'désactiver' : 'activer';
    if (!confirm(`Voulez-vous ${action} le compte de ${user.prenom} ${user.nom} ?`)) return;

    this.http.patch(`${this.apiUrl}/${user.utilisateurId}/active`, !user.actif).subscribe({
      next: () => {
        this.successMessage = `Compte ${user.actif ? 'désactivé' : 'activé'} avec succès.`;
        this.loadUsers();
      },
      error: () => {
        this.errorMessage = 'Erreur lors de la modification du statut.';
      }
    });
  }

  deleteUser(user: UserDto): void {
    if (!confirm(`Supprimer définitivement le compte de ${user.prenom} ${user.nom} ?`)) return;

    this.http.delete(`${this.apiUrl}/${user.utilisateurId}`).subscribe({
      next: () => {
        this.successMessage = 'Utilisateur supprimé.';
        this.loadUsers();
      },
      error: () => {
        this.errorMessage = 'Erreur lors de la suppression.';
      }
    });
  }

  get nomControl() { return this.userForm.get('nom'); }
  get prenomControl() { return this.userForm.get('prenom'); }
  get emailControl() { return this.userForm.get('email'); }
  get motDePasseControl() { return this.userForm.get('motDePasse'); }
}