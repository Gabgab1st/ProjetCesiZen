// src/app/shared/models/exercice.model.ts
export interface Exercice {
  exerciceId: number;
  nom: string;
  dureeInspiration: number;
  dureeApnee: number;
  dureeExpiration: number;
  actif: boolean;
}