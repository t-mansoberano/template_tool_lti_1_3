import {inject, Injectable, signal} from '@angular/core';
import {ViewModel} from '../models/view.model';
import {ApiService} from './api.service';
import {AuthService} from '../../../../core/services/auth.service';
import {IBmbTab} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {ActivatedRoute, Router} from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class StateService {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  private authService = inject(AuthService);
  private apiService = inject(ApiService);

  private _viewModel = signal<ViewModel | null>(null);
  private _loading = signal(true);
  private _error = signal<string | null>(null);

  get viewModel() {
    return this._viewModel.asReadonly();
  }

  get loading() {
    return this._loading.asReadonly();
  }

  get error() {
    return this._error.asReadonly();
  }

  tabs: IBmbTab[] = [
    {id: 1, title: 'Evaluar por alumnos', isActive: true},
    {id: 2, title: 'Evaluar por competencia/subcompetencia'},
  ];

  load(): void {
    const courseId = this.authService.getCourseId();
    this._loading.set(true);
    this.apiService.getEvaluations(courseId).subscribe({
      next: (response) => {
        this._viewModel.set(response);
        this.clearError();
      },
      error: (err) => {
        this.setError('Error al cargar las evaluaciones.');
      },
      complete: () => {
        this._loading.set(false);
      },
    });
  }

  selectStudent(studentId: string): void {
    this._loading.set(true);
    this.apiService.getStudentCourseEvaluations(this.authService.getCourseId(), studentId).subscribe({
      next: (response) => {
        const currentViewModel = this._viewModel();
        this._viewModel.set({...currentViewModel, selectedStudent: response.selectedStudent} as ViewModel);
      },
      error: (err) => {
        this.setError('Error al cargar los datos del alumno.');
      },
      complete: () => {
        this._loading.set(false);
      },
    });
  }

  setError(message: string): void {
    this._error.set(message);
  }

  clearError(): void {
    this._error.set(null);
  }

  navigate(tab: IBmbTab) {
    this.router.navigate(['instructor-competencies'], {relativeTo: this.route.parent});
  }

}
