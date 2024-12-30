import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isAuthenticated$ = new BehaviorSubject<boolean>(false);

  login(username: string, password: string): void {
    // Simular inicio de sesión (reemplazar con lógica real)
    if (username && password) {
      this.isAuthenticated$.next(true);
    }
  }

  logout(): void {
    this.isAuthenticated$.next(false);
  }

  isLoggedIn(): Observable<boolean> {
    return this.isAuthenticated$.asObservable();
  }
}
