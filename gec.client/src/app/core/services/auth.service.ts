import {inject, Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {catchError, tap} from 'rxjs/operators';
import {ApiService} from './api.service';
import {LtiContext, Resolve} from '../models/lti-context.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiService = inject(ApiService);

  private _isInstructor = false;
  private _isStudent = false;
  private _isWithoutRole = false;
  private _isError = false;

  getLtiContext(): Observable<Resolve> {
    return this.apiService.get<Resolve>('/api/lti').pipe(
      tap((context: Resolve) => {
        this._isInstructor = context.result.user.isInstructor;
        this._isStudent = context.result.user.isStudent;
        this._isWithoutRole = context.result.user.isWithoutRole;
        this._isError = false;
      }),
      catchError((err) => {
        this._isInstructor = false;
        this._isStudent = false;
        this._isWithoutRole = false;
        this._isError = true;
        throw err;
      }),
    );
  }

  isInstructor(): boolean {
    return this._isInstructor;
  }

  isStudent(): boolean {
    return this._isStudent;
  }

  isWithoutRole(): boolean {
    return this._isWithoutRole;
  }

  isError(): boolean {
    return this._isError;
  }

}
