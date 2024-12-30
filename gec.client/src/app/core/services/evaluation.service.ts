import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

@Injectable({
  providedIn: 'root'
})
export class EvaluationService {
  private readonly baseUrl = '/api/evaluations';

  constructor(private api: ApiService) {}

  getEvaluations(): Observable<any[]> {
    return this.api.get<any[]>(`${this.baseUrl}`);
  }

  getEvaluationById(id: string): Observable<any> {
    return this.api.get<any>(`${this.baseUrl}/${id}`);
  }

  saveEvaluation(evaluation: any): Observable<any> {
    return this.api.post<any>(`${this.baseUrl}`, evaluation);
  }

  updateEvaluation(id: string, evaluation: any): Observable<any> {
    return this.api.put<any>(`${this.baseUrl}/${id}`, evaluation);
  }

  deleteEvaluation(id: string): Observable<any> {
    return this.api.delete<any>(`${this.baseUrl}/${id}`);
  }
}
