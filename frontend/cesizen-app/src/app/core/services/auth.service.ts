import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, User } from '../../shared/models/user.model';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:5030';
  redirectUrl: string | null = null; 
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient) {
  try {
    const stored = localStorage.getItem('currentUser');
    if (stored) this.currentUserSubject.next(JSON.parse(stored));
  } catch {
    localStorage.removeItem('currentUser');
  }
}

  login(request: LoginRequest): Observable<AuthResponse> {
  return this.http.post<AuthResponse>(`${this.apiUrl}/api/auth/login`, request).pipe(
    tap(response => {
      localStorage.setItem('token', response.token);
      // Construire l'objet utilisateur depuis la réponse
      const user = {
        email: response.email,
        nomComplet: response.nomComplet,
        role: response.role,
        prenom: response.nomComplet?.split(' ')[0] ?? ''
      };
      localStorage.setItem('currentUser', JSON.stringify(user));
      this.currentUserSubject.next(user as any);
    })
  );
}

  register(request: RegisterRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/api/auth/register`, request);
  }
resetPassword(dto: { email: string; nouveauMotDePasse: string }): Observable<any> {
  return this.http.post(`${this.apiUrl}/api/auth/reset-password`, dto);
}
  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('currentUser');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
  return this.currentUserSubject.value?.role === 'Administrateur';
}
}