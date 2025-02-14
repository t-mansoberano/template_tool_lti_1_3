import { Injectable } from '@angular/core';
import { Resolve } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { UserClaims } from '../models/userclaims.entity';


@Injectable({
  providedIn: 'root'
})
export class UserClaimsResolver implements Resolve<UserClaims> {
  constructor(private authService: AuthService) {}

  resolve(): Observable<UserClaims> {
    return this.authService.getUserClaims();
  }
}