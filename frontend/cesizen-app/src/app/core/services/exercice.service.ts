import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Exercice } from '../../shared/models/exercice.model';

@Injectable({ providedIn: 'root' })
export class ExerciceService {
  private apiUrl = 'http://localhost:5030/api/exercicesrespiration';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Exercice[]> {
    return this.http.get<Exercice[]>(this.apiUrl);
  }

  getById(id: number): Observable<Exercice> {
    return this.http.get<Exercice>(`${this.apiUrl}/${id}`);
  }
}