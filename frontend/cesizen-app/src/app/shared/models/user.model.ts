// src/app/shared/models/user.model.ts
export interface User {
  utilisateurId: number;
  nom: string;
  prenom: string;
  email: string;
  actif: boolean;
  createdAt: Date;
  roleId: number;
  role?: string;
}

export interface LoginRequest {
  email: string;
  motDePasse: string;
}

export interface RegisterRequest {
  nom: string;
  prenom: string;
  email: string;
  motDePasse: string;
}

export interface AuthResponse {
  token: string;
  utilisateur: User;
}