// src/app/shared/models/menu.model.ts
import { Page } from './page.model';

export interface Menu {
  menuId: number;
  ordre: number;
  libelle: string;
  pages?: Page[];
}