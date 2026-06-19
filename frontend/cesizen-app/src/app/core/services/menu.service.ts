import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Menu } from '../../shared/models/menu.model';

@Injectable({ providedIn: 'root' })
export class MenuService {
  private apiUrl = 'http://localhost:5030/api/menus';

  constructor(private http: HttpClient) {}

  getAll(): Observable<Menu[]> {
    return this.http.get<Menu[]>(this.apiUrl);
  }

  getById(id: number): Observable<Menu> {
    return this.http.get<Menu>(`${this.apiUrl}/${id}`);
  }
}