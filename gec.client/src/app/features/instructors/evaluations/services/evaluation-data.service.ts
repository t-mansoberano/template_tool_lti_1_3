import {inject, Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';
import {ApiService} from '../../../../core/services/api.service';
import {AuthService} from '../../../../core/services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class EvaluationDataService {
  private apiService = inject(ApiService); // Uso de inject para inyectar ApiService
  private authService = inject(AuthService); // Uso de inject para inyectar ApiService

  getTestCanvasAPI(): Observable<any> {
    const courseId = this.authService.getCourseId();
    const canvasApiUrl = `/api/teachers/courses/${courseId}/testapicanvas`;
    return this.apiService.get<any>(canvasApiUrl);
  }

  getTestEvaluations(): Observable<any> {
    const courseId = this.authService.getCourseId();
    const canvasApiUrl = `/api/teachers/courses/${courseId}/evaluations`;
    return this.apiService.get<any>(canvasApiUrl);
  }

}
