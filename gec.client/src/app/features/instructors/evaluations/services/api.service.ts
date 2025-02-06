import {inject, Injectable} from '@angular/core';
import {map, Observable} from 'rxjs';
import {HttpService} from '../../../../core/services/http.service';
import {ViewModel, ViewStudentEvidencesModel} from '../models/view.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private httpService = inject(HttpService);

  getTestCanvasAPI(courseId: string): Observable<any> {
    const canvasApiUrl = `/api/teachers/courses/${courseId}/testapicanvas`;
    return this.httpService.get(canvasApiUrl);
  }

  getStudentCourseEvaluations(courseId: string, studentId: string): Observable<ViewStudentEvidencesModel> {
    const canvasApiUrl = `/api/teachers/courses/${courseId}/students/${studentId}/evaluations`;
    return this.httpService.get(canvasApiUrl).pipe(
      map(response => response.result as ViewStudentEvidencesModel)
    );
  }

  getEvaluations(courseId: string): Observable<ViewModel> {
    const url = `/api/teachers/courses/${courseId}/evaluations`;
    return this.httpService.get(url).pipe(
      map(response => response.result as ViewModel)
    );
  }

}
