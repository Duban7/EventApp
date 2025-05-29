import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  imports: [CommonModule],
  template: `
    <div *ngIf="authService.currentUserValue">
      <h2>Профиль пользователя</h2>
      <p>ID: {{ authService.userId }}</p>
      <p>Email: {{ authService.currentUserValue.email }}</p>
      <p>Is admin: {{authService.isAdmin}}</p>
      <button (click)="logout()">Выйти</button>
    </div>
  `
})
export class ProfileComponent implements OnInit {
  constructor(public authService: AuthService) {}

  ngOnInit(): void {
    if (!this.authService.isLoggedIn) {
      // Перенаправление, если не авторизован
      this.authService.logout();
    }
  }

  logout(): void {
    this.authService.logout();
  }
}