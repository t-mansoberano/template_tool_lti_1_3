import {inject, Injectable} from '@angular/core';
import {map, Observable} from 'rxjs';
import {catchError, tap} from 'rxjs/operators';
import {ApiService} from './api.service';
import {Resolve} from '../models/resolve.model';
import {Context} from '../models/context.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiService = inject(ApiService);

  private _isInstructor = false;
  private _isStudent = false;
  private _isWithoutRole = false;
  private _isError = false;
  private _isExternalCollaborator = false;
  private _courseId = "";

  getLtiContext(): Observable<Context> {
    return this.apiService.get('/api/lti').pipe(
      map((respondModel) => respondModel.result as Context),
      tap((context) => {
        this._courseId = context.course.id;
        this._isInstructor = context.user.isInstructor;
        this._isStudent = context.user.isStudent;
        this._isExternalCollaborator = context.user.isExternalCollaborator;
        this._isWithoutRole = context.user.isWithoutRole;
        this._isError = false;
      }),
      catchError((err) => {
        console.error('Error en LTI, intentando Federación...', err);
        return this.getFederationContext();
      }),
    );
  }

  getFederationContext(): Observable<Context> {
    return this.apiService.get('/api/federation').pipe(
      map((respondModel) => respondModel.result as Context),
      tap((context) => {
        this._courseId = "";
        this._isInstructor = context.user.isInstructor;
        this._isStudent = context.user.isStudent;
        this._isExternalCollaborator = context.user.isExternalCollaborator;
        this._isWithoutRole = context.user.isWithoutRole;
        this._isError = false;
      }),
      catchError((err) => {
        this._isInstructor = false;
        this._isStudent = false;
        this._isExternalCollaborator = false;
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

  isExternalCollaborator(): boolean {
    return this._isExternalCollaborator;
  }

  isWithoutRole(): boolean {
    return this._isWithoutRole;
  }

  isError(): boolean {
    return this._isError;
  }

  getCourseId(): string {
    return this._courseId;
  }

}
