import {inject, Injectable} from '@angular/core';
import {map, Observable, throwError} from 'rxjs';
import {catchError, tap} from 'rxjs/operators';
import {HttpService} from './http.service';
import {Resolve} from '../models/resolve.model';
import {Context} from '../models/context.model';
import { UserClaims } from '../models/userclaims.entity';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiService = inject(HttpService);

  private _isInstructor = false;
  private _isStudent = false;
  private _isWithoutRole = false;
  private _isError = false;
  private _isExternalCollaborator = false;
  private _courseId = "";

  getContext(): Observable<Context> {
    // Detectar si la URL contiene parámetros LTI para decidir el flujo
    const isLtiLaunch = this.isEmbeddedInCanvas();

    if (isLtiLaunch) {
      console.log('Detectado acceso desde Canvas (LTI), cargando contexto LTI...');
      return this.getLtiContext();
    } else {
      console.log('Acceso normal, cargando contexto federado...');
      return this.getFederationContext();
    }
  }

  private isEmbeddedInCanvas(): boolean {
    // Si en la URL aparece ?forceEmbeddedInCanvas=true, forzamos acceso normal
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('forceEmbeddedInCanvas') === 'true') {
      return true;
    }

    // Verifica si la aplicación está embebida dentro de Canvas LMS
    return window.location.ancestorOrigins?.[0]?.includes('instructure.com') || false;
  }


  getUserClaims(): Observable<UserClaims> {
    return this.apiService.get('/api/Home/GetUserClaims').pipe(
      map((respondModel) => {
        const userClaims = respondModel.result as UserClaims;
        console.log('User Claims:', respondModel);
        return userClaims;
      }),
      catchError((err) => {
        console.error('Error fetching user claims', err);
        throw err;
      })
    );
  }


  private getLtiContext(): Observable<Context> {
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
        console.error('Error en LTI, abortando...', err);
        this._isError = true;
        return throwError(() => new Error('No se pudo autenticar via LTI'));
      }),
    );
  }

  private getFederationContext(): Observable<Context> {
    return this.apiService.get('/api/federation').pipe(
      map((respondModel) => respondModel.result as Context),
      tap((context) => {
        this._courseId = '';
        this._isInstructor = context.user.isInstructor;
        this._isStudent = context.user.isStudent;
        this._isExternalCollaborator = context.user.isExternalCollaborator;
        this._isWithoutRole = context.user.isWithoutRole;
        this._isError = false;
      }),
      catchError((err) => {
        console.error('Error en Federación', err);
        this._isError = true;
        return throwError(() => new Error('No se pudo autenticar via Federación'));
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
