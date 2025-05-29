import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  imports:[CommonModule]
})
export class HeaderComponent {
  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  navigateToProfile(): void {
    if (this.authService.isLoggedIn) {
      this.router.navigate(['/profile']);
    } else {
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: '/profile' } 
      });
    }
  }

  navigateToAdmin(): void {
    if (this.authService.isAdmin) {
      this.router.navigate(['/admin']);
    }
  }

  navigateHome(): void{
    this.router.navigate(['/']);
  }
}