import { Component, input } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import {AuthService} from '../../../core/services/auth.service'
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-login',
  imports:[ReactiveFormsModule, CommonModule],
  templateUrl: './login.component.html',
  styleUrl:'./login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.isLoading = true;
    this.errorMessage = '';

    const { email, password } = this.loginForm.value;

    this.authService.logIn(email, password).subscribe({
      next: () => {
        const returnUrl = this.router.parseUrl(this.router.url).queryParams['returnUrl'] || '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Ошибка входа';
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }
  navigateRegister(){
    this.router.navigate(['/register'])
  }
}