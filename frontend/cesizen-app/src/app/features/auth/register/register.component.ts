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

// Validateur personnalisé : vérifie que les deux mots de passe correspondent
function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const motDePasse = control.get('motDePasse');
  const confirmation = control.get('confirmation');
  if (!motDePasse || !confirmation) return null;
  return motDePasse.value === confirmation.value ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
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
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  loading = false;
  errorMessage = '';
  successMessage = '';
  hidePassword = true;
  hideConfirmation = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
    }

    this.registerForm = this.fb.group(
      {
        nom: ['', [Validators.required, Validators.minLength(2)]],
        prenom: ['', [Validators.required, Validators.minLength(2)]],
        email: ['', [Validators.required, Validators.email]],
        motDePasse: ['', [Validators.required, Validators.minLength(8)]],
        confirmation: ['', Validators.required],
      },
      { validators: passwordMatchValidator }
    );
  }

  get nomControl() { return this.registerForm.get('nom'); }
  get prenomControl() { return this.registerForm.get('prenom'); }
  get emailControl() { return this.registerForm.get('email'); }
  get motDePasseControl() { return this.registerForm.get('motDePasse'); }
  get confirmationControl() { return this.registerForm.get('confirmation'); }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const { nom, prenom, email, motDePasse } = this.registerForm.value;

    this.authService.register({ nom, prenom, email, motDePasse }).subscribe({
      next: () => {
        this.loading = false;
        this.successMessage = 'Compte créé avec succès ! Redirection vers la connexion…';
        setTimeout(() => this.router.navigate(['/auth/login']), 2000);
      },
      error: (err) => {
        this.loading = false;
        if (err.status === 409) {
          this.errorMessage = 'Cette adresse e-mail est déjà utilisée.';
        } else {
          this.errorMessage = 'Une erreur est survenue. Veuillez réessayer.';
        }
      },
    });
  }
}