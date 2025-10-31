import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '@core/services/auth.service';

import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { LoginRequestDto } from '@shared/models/auth.model';
// ------------------------------------------------

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  public isLoading = signal(false);
  public loginError = signal<string | null>(null);
  public hidePassword = signal(true);

  loginForm = this.fb.group({
    email: ['admin@example.com', [Validators.required, Validators.email]],
    password: ['admin123', [Validators.required]]
  });

  onSubmit() {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading.set(true);
    this.loginError.set(null);

    const request = this.loginForm.value as LoginRequestDto;

    this.authService.login(request).subscribe({
      next: () => {
        // Sucesso!
        this.isLoading.set(false);
        this.router.navigate(['/tasks']);
      },
      error: (err) => {
        this.isLoading.set(false);
        if (err.status === 400) {
          this.loginError.set(err.error.error || 'Email ou senha inválidos.');
        } else {
          this.loginError.set('Ocorreu um erro no servidor. Tente novamente.');
        }
      }
    });
  }
}