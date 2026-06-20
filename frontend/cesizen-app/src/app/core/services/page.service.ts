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
    return this.http.get<Page>(`${this.apiUrl}/${slug}`);
  }

  getById(id: number): Observable<Page> {
    return this.http.get<Page>(`${this.apiUrl}/${id}`);
  }

  create(page: Partial<Page>): Observable<Page> {
    return this.http.post<Page>(this.apiUrl, page);
  }

  update(id: number, page: Partial<Page>): Observable<Page> {
    return this.http.put<Page>(`${this.apiUrl}/${id}`, page);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}