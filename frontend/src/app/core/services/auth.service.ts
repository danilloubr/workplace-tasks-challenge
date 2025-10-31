import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { LoginRequestDto, LoginResponseDto } from '@shared/models/auth.model';
import { environment } from '@env/environment';

import { StorageService } from './storage.service';
import { StorageKeys } from '@shared/constants/storage-keys';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private storageService = inject(StorageService);

  private apiUrl = `${environment.apiUrl}/auth`;

  public isLoggedIn = signal<boolean>(this.hasToken());

  login(loginDto: LoginRequestDto): Observable<LoginResponseDto> {
    return this.http.post<LoginResponseDto>(`${this.apiUrl}/login`, loginDto).pipe(
      tap((response) => {
        this.storageService.saveItem(StorageKeys.AUTH_TOKEN, response.token);

        this.isLoggedIn.set(true);
      })
    );
  }

  logout(): void {
    this.storageService.removeItem(StorageKeys.AUTH_TOKEN);
    this.isLoggedIn.set(false);
  }

  getToken(): string | null {
    return this.storageService.getItem<string>(StorageKeys.AUTH_TOKEN);
  }

  private hasToken(): boolean {
    return !!this.storageService.getItem(StorageKeys.AUTH_TOKEN);
  }
}
