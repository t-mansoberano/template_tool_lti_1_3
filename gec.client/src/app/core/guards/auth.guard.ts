import {inject} from '@angular/core';
import {CanActivateFn, Router} from '@angular/router';
import {AuthService} from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isError()) {
    router.navigate(['/error']);
    return false;
  }

  if (route.routeConfig?.path === 'instructor-view' && !authService.isInstructor()) {
    router.navigate(['/error']);
    return false;
  }

  if (route.routeConfig?.path === 'student-view' && !authService.isStudent()) {
    router.navigate(['/error']);
    return false;
  }

  if (route.routeConfig?.path === 'external-collaborator-view' && !authService.isExternalCollaborator()) {
    router.navigate(['/error']);
    return false;
  }

  return true; // Permite el acceso si cumple con el rol
};
