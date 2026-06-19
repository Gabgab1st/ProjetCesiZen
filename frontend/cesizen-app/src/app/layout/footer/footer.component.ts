// src/app/layout/footer/footer.component.ts
import { Component } from '@angular/core';
import { MatToolbarModule } from '@angular/material/toolbar';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [MatToolbarModule],
  template: `
    <mat-toolbar class="footer">
      <span>© 2024 CESIZen — L'application de votre santé mentale</span>
    </mat-toolbar>
  `,
  styles: [`
    .footer {
      position: fixed;
      bottom: 0;
      width: 100%;
      justify-content: center;
      font-size: 0.85rem;
      opacity: 0.9;
    }
  `]
})
export class FooterComponent {}