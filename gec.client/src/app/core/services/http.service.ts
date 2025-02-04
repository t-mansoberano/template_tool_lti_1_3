import {inject, Injectable} from '@angular/core';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {map, Observable, throwError} from 'rxjs';
import {RespondModel} from '../models/respond.model';
import {catchError} from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private http = inject(HttpClient);

  get(url: string): Observable<RespondModel> {
    return this.http.get<RespondModel>(url, {observe: 'response'}).pipe(
      map(response => response.body as RespondModel),
      catchError(error => this.handleHttpError(error))
    );
  }

  post(url: string, body: any): Observable<RespondModel> {
    return this.http.post<RespondModel>(url, body).pipe(
      catchError(error => this.handleHttpError(error))
    );
  }

  put(url: string, body: any): Observable<RespondModel> {
    return this.http.put<RespondModel>(url, body).pipe(
      catchError(error => this.handleHttpError(error))
    );
  }

  delete(url: string): Observable<RespondModel> {
    return this.http.delete<RespondModel>(url).pipe(
      catchError(error => this.handleHttpError(error))
    );
  }

  private handleHttpError = (error: HttpErrorResponse): Observable<RespondModel> => {
    let dataError = (error?.error?.errorMessage ? error.error : {errorMessage: error.message}) as RespondModel;
    return throwError(() => dataError);
  };

}
