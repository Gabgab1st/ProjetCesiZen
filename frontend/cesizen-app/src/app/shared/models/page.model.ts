// src/app/shared/models/page.model.ts
export interface Page {
  pageId: number;
  titre: string;
  contenu: string;
  slug: string;
  isPublic: boolean;
  updatedAt: Date;
  menuId: number;
}