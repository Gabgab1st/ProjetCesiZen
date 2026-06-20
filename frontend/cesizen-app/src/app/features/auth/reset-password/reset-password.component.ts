import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../../core/services/auth.service';

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const nouveau = control.get('nouveauMotDePasse');
  const confirmation = control.get('confirmation');
  if (!nouveau || !confirmation) return null;
  return nouveau.value === confirmation.value ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
  resetForm!: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  hideNew = true;
  hideConfirm = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Redirige si non connecté
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login']);
      return;
    }

    // Pré-remplit l'email depuis l'utilisateur courant
    const email = this.authService.currentUser$
      ? ''
      : '';

    this.resetForm = this.fb.group(
      {
        email: [
          { value: this.getCurrentEmail(), disabled: true },
          [Validators.required, Validators.email],
        ],
        nouveauMotDePasse: ['', [Validators.required, Validators.minLength(8)]],
        confirmation: ['', Validators.required],
      },
      { validators: passwordMatchValidator }
    );
  }

  private getCurrentEmail(): string {
    let email = '';
    this.authService.currentUser$.subscribe(user => {
      email = user?.email ?? '';
    }).unsubscribe();
    return email;
  }

  get nouveauControl() { return this.resetForm.get('nouveauMotDePasse'); }
  get confirmationControl() { return this.resetForm.get('confirmation'); }

  onSubmit(): void {
    if (this.resetForm.invalid) {
      this.resetForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const email = this.resetForm.getRawValue().email;
    const nouveauMotDePasse = this.resetForm.value.nouveauMotDePasse;

    this.authService.resetPassword({ email, nouveauMotDePasse }).subscribe({
      next: () => {
        this.loading = false;
        this.successMessage = 'Mot de passe modifié avec succès ! Veuillez vous reconnecter.';
        setTimeout(() => {
          this.authService.logout();
          this.router.navigate(['/auth/login']);
        }, 2000);
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Une erreur est survenue. Veuillez réessayer.';
      },
    });
  }
}