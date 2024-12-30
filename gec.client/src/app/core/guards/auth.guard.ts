import {inject} from '@angular/core';
import {map} from 'rxjs/operators';
import {CanActivateFn, Router} from '@angular/router';
import {AuthService} from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isLoggedIn().pipe(map((isLoggedIn) => {
    if (!isLoggedIn) {
      router.navigate(['/login']);
      return false;
    }
    return true;
  }));
};
