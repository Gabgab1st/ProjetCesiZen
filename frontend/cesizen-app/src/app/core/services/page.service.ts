import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Page } from '../../shared/models/page.model';

@Injectable({ providedIn: 'root' })
export class PageService {
  private apiUrl = 'http://localhost:5030/api/pages';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Page[]> {
    return this.http.get<Page[]>(this.apiUrl);
  }

  getBySlug(slug: string): Observable<Page> {
    return this.http.get<Page>(`${this.apiUrl}/slug/${slug}`);
  }

  getById(id: number): Observable<Page> {
    return this.http.get<Page>(`${this.apiUrl}/${id}`);
  }
}