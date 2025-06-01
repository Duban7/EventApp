import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = 'https://localhost:8081/EventApi/user';

  constructor(private http: HttpClient) {}

  getUser(userId: string): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/get/${userId}`);
  }

  updateUser(userId: string, userData: Partial<User>): Observable<User> {
    console.log(userData);
    const formData = new FormData();
    formData.append('email', userData.email!);
    formData.append('name', userData.name!);
    formData.append('surname', userData.surname!);
    formData.append('birthDate', userData.birthDate!.toString());

    return this.http.put<User>(`${this.apiUrl}/update`, formData);
  }

  deleteUser(userId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/delete`);
  }
}
