import {inject, Injectable} from '@angular/core';
import {map, Observable} from 'rxjs';
import {ApiService} from '../../../../core/services/api.service';
import {AuthService} from '../../../../core/services/auth.service';
import {ViewModel} from '../models/view.model';

@Injectable({
  providedIn: 'root'
})
export class EvaluationDataService {
  private apiService = inject(ApiService); // Uso de inject para inyectar ApiService
  private authService = inject(AuthService); // Uso de inject para inyectar ApiService

  getTestCanvasAPI(): Observable<any> {
    const courseId = this.authService.getCourseId();
    const canvasApiUrl = `/api/teachers/courses/${courseId}/testapicanvas`;
    return this.apiService.get(canvasApiUrl);
  }

  getTestEvaluations(): Observable<ViewModel> {
    const courseId = this.authService.getCourseId();
    const canvasApiUrl = `/api/teachers/courses/${courseId}/evaluations`;
    return this.apiService.get(canvasApiUrl).pipe(
      map((respondModel) => respondModel.result as ViewModel),
    )
  }

}
