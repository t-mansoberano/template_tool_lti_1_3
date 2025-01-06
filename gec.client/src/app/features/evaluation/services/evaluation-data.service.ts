import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import {ApiService} from '../../../core/services/api.service';
import {Evaluation} from '../models/evaluation.model';
import {AuthService} from '../../../core/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class EvaluationDataService {
  private apiService = inject(ApiService); // Uso de inject para inyectar ApiService
  private authService = inject(AuthService); // Uso de inject para inyectar ApiService

  getEvaluations(): Observable<Evaluation> {
    return of({
      students: [
        {id: 'A000101', name: 'Raquel Dooley', evaluated: 2, total: 10},
        {id: 'A000102', name: 'James Kuphal', evaluated: 4, total: 10},
        {id: 'A000103', name: 'Joanna Windler', evaluated: 1, total: 10},
      ],
      feedbacks: [
        {title: 'Evidencia 1', description: 'Detalles de la evidencia 1.', score: 'Sólido'},
        {title: 'Evidencia 2', description: 'Detalles de la evidencia 2.', score: 'Básico'},
      ],
      competencies: [
        {name: 'Competencia 1', description: 'Diseña mecanismos de control interno.'},
        {name: 'Competencia 2', description: 'Evalúa sistemas administrativos.'},
      ]
    });
  }

  getTestCanvasAPI(): Observable<any> {
    const courseId = this.authService.getCourseId();
    const canvasApiUrl = `/api/teachers/courses/${courseId}/testapicanvas`;
    return this.apiService.get<any>(canvasApiUrl);
  }

}
