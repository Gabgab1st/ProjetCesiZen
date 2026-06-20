import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';

import { ExerciceService } from '../../../core/services/exercice.service';
import { Exercice } from '../../../shared/models/exercice.model';

type Phase = 'inspiration' | 'apnee' | 'expiration' | 'repos';

@Component({
  selector: 'app-exercice-detail',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatFormFieldModule,
  ],
  templateUrl: './exercice-detail.component.html',
  styleUrls: ['./exercice-detail.component.scss'],
})
export class ExerciceDetailComponent implements OnInit, OnDestroy {
  exercices: Exercice[] = [];
  selectedExercice: Exercice | null = null;
  loading = true;
  errorMessage = '';

  // État de l'animation
  isRunning = false;
  currentPhase: Phase = 'repos';
  phaseLabel = 'Prêt';
  countdown = 0;
  cycleCount = 0;

  private timer: ReturnType<typeof setTimeout> | null = null;

  constructor(private exerciceService: ExerciceService) {}

  ngOnInit(): void {
    this.exerciceService.getAll().subscribe({
      next: (data) => {
        this.exercices = data.filter(e => e.actif);
        if (this.exercices.length > 0) {
          this.selectedExercice = this.exercices[0];
        }
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.errorMessage = 'Impossible de charger les exercices.';
      },
    });
  }

  ngOnDestroy(): void {
    this.stopExercice();
  }

  onSelectExercice(exercice: Exercice): void {
    this.stopExercice();
    this.selectedExercice = exercice;
  }

  startExercice(): void {
    if (!this.selectedExercice) return;
    this.isRunning = true;
    this.cycleCount = 0;
    this.runPhase('inspiration');
  }

  stopExercice(): void {
    this.isRunning = false;
    this.currentPhase = 'repos';
    this.phaseLabel = 'Prêt';
    this.countdown = 0;
    if (this.timer) {
      clearTimeout(this.timer);
      this.timer = null;
    }
  }

  private runPhase(phase: Phase): void {
    if (!this.isRunning || !this.selectedExercice) return;

    this.currentPhase = phase;
    let duree = 0;

    switch (phase) {
      case 'inspiration':
        this.phaseLabel = 'Inspirez';
        duree = this.selectedExercice.dureeInspiration;
        break;
      case 'apnee':
        this.phaseLabel = 'Retenez';
        duree = this.selectedExercice.dureeApnee;
        break;
      case 'expiration':
        this.phaseLabel = 'Expirez';
        duree = this.selectedExercice.dureeExpiration;
        break;
    }

    // Si apnée = 0, on la saute
    if (phase === 'apnee' && duree === 0) {
      this.runPhase('expiration');
      return;
    }

    this.countdown = duree;
    this.tick(phase, duree);
  }

  private tick(phase: Phase, remaining: number): void {
    if (!this.isRunning) return;

    this.countdown = remaining;

    if (remaining <= 0) {
      // Passage à la phase suivante
      switch (phase) {
        case 'inspiration':
          this.runPhase('apnee');
          break;
        case 'apnee':
          this.runPhase('expiration');
          break;
        case 'expiration':
          this.cycleCount++;
          this.runPhase('inspiration');
          break;
      }
      return;
    }

    this.timer = setTimeout(() => {
      this.tick(phase, remaining - 1);
    }, 1000);
  }

  // Durée totale d'un cycle pour l'affichage
  get cycleDuree(): number {
    if (!this.selectedExercice) return 0;
    return (
      this.selectedExercice.dureeInspiration +
      this.selectedExercice.dureeApnee +
      this.selectedExercice.dureeExpiration
    );
  }
}