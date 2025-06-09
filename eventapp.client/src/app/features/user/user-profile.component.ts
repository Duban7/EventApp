import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { UserService } from '../../core/services/user.service';
import { EventService } from '../../core/services/event.service';
import { Router, RouterModule } from '@angular/router';
import { User } from '../../core/models/user';
import { Event } from '../../core/models/event';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss'],
  imports:[ReactiveFormsModule, CommonModule, RouterModule]
})
export class ProfileComponent implements OnInit {
  user: User | null = null;
  userEvents: Event[] = [];
  isEditing = false;
  showDeleteConfirmation = false;
  isLoading = false;
  errorMessage = '';
  isAdmin = false;
  yesterday = new Date(new Date().setDate(new Date().getDate() - 1)).toISOString().split('T')[0];

  profileForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private userService: UserService,
    private eventService: EventService,
    private router: Router
  ) {
    this.profileForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(30)]],
      surname: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email]],
      birthDate: ['', [Validators.required]]
    });
  }

  ngOnInit(): void {
    this.loadUserData();
    this.loadUserEvents();
  }

  loadUserData(): void {
    this.isLoading = true;
    this.authService.currentUser.subscribe(user => {
      this.user = user;
      if (user) {
        this.profileForm.patchValue({
          name: user.name,
          surname: user.surname,
          email: user.email,
          birthDate: user.birthDate ? new Date(new Date().setDate(new Date(user.birthDate).getDate())).toISOString().split('T')[0] : null
        });
        this.isAdmin = this.authService.isAdmin;
      }
      this.isLoading = false;
    });
  }

  loadUserEvents(): void {
    if (this.authService.userId) {
      this.eventService.getUserEvents(this.authService.userId).subscribe(events => {
        this.userEvents = events;
      });
    }
  }

  startEditing(): void {
    this.isEditing = true;
    this.profileForm.get('name')?.enable();
    this.profileForm.get('surname')?.enable();
    this.profileForm.get('birthDate')?.enable();
  }

  cancelEditing(): void {
    this.isEditing = false;
    if (this.user) {
      this.profileForm.patchValue({
        name: this.user.name,
        surname: this.user.surname,
        birthDate: this.user.birthDate ? new Date(new Date().setDate(new Date(this.user.birthDate).getDate())).toISOString().split('T')[0] : null
      });
    }
    this.profileForm.get('name')?.disable();
    this.profileForm.get('surname')?.disable();
    this.profileForm.get('birthDate')?.disable();
  }

  saveChanges(): void {
    if (this.profileForm.invalid || !this.user) return;
    console.log(this.profileForm.value);
    this.isLoading = true;
    const {email, name, surname, birthDate } = this.profileForm.value;

    this.userService.updateUser(this.user.id, {
      email:email,
      name: name,
      surname: surname,
      birthDate: birthDate
    }).subscribe({
      next: (updatedUser) => {
        this.authService.updateCurrentUser(updatedUser);
        this.isEditing = false;
        this.isLoading = false;
        this.errorMessage = '';
      },
      error: (err) => {
        this.errorMessage = 'Ошибка при обновлении данных';
        this.isLoading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }

  confirmDelete(): void {
    this.showDeleteConfirmation = true;
  }

  deleteAccount(): void {
    if (!this.user) return;

    this.isLoading = true;
    this.userService.deleteUser(this.user.id).subscribe({
      next: () => {
        this.authService.logout();
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.errorMessage = 'Ошибка при удалении аккаунта';
        this.isLoading = false;
        this.showDeleteConfirmation = false;
      }
    });
  }

  cancelDelete(): void {
    this.showDeleteConfirmation = false;
  }
}
