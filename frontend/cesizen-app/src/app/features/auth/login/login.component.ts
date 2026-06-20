import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
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
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  loading = false;
  errorMessage = '';
  hidePassword = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Redirige si déjà connecté
  if (this.authService.isLoggedIn()) {
        this.router.navigate(['/']);
    }

    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      motDePasse: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  get emailControl() {
    return this.loginForm.get('email');
  }

  get motDePasseControl() {
    return this.loginForm.get('motDePasse');
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const { email, motDePasse } = this.loginForm.value;

    this.authService.login({ email, motDePasse }).subscribe({
      next: () => {
        this.loading = false;
        // Redirige vers la page précédente ou l'accueil
        const redirect = this.authService.redirectUrl || '/';
        this.router.navigateByUrl(redirect);
      },
      error: (err) => {
        this.loading = false;
        if (err.status === 401) {
          this.errorMessage = 'Email ou mot de passe incorrect.';
        } else if (err.status === 403) {
          this.errorMessage = 'Votre compte est désactivé. Contactez un administrateur.';
        } else {
          this.errorMessage = 'Une erreur est survenue. Veuillez réessayer.';
        }
      },
    });
  }
}