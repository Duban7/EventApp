import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './register.component.html', 
  styleUrl: './register.component.scss' 
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  yesterday = new Date(new Date().setDate(new Date().getDate() - 1)).toISOString().split('T')[0];


  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router : Router
  ) {
    this.registerForm = this.fb.group({
      name: ['', Validators.required],
      surname: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      dateOfBirth: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.registerForm.invalid) return;

    this.isLoading = true;
    
    const formValue = {
      ...this.registerForm.value,
      dateOfBirth: new Date(this.registerForm.value.dateOfBirth)
    };

    this.authService.register(formValue).subscribe({
      next: () => {
        const returnUrl = this.router.parseUrl(this.router.url).queryParams['returnUrl'] || '/';
        this.router.navigateByUrl(returnUrl);
      },
      error: (err) => {
        let msg;
        if(err.status == 409) msg = "Пользователь с таким email уже существует" ;
        if(err.status == 400) msg = "Пользователь не прошел валидацию" ;
        this.errorMessage = err.error?.message || 'Ошибка регистрации: '+msg;
        this.isLoading = false;
      }
    });
  }
}
