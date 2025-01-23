import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit} from '@angular/core';
import {CourseSummaryComponent} from './components/course-summary/course-summary.component';
import {EvaluationStatusComponent} from './components/evaluation-status/evaluation-status.component';
import {StudentListComponent} from './components/student-list/student-list.component';
import {StudentDetailComponent} from './components/student-detail/student-detail.component';
import {BmbLoaderComponent, BmbTabsComponent, IBmbTab} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {StateService} from './services/state.service';

@Component({
  selector: 'app-evaluation',
  standalone: true,
  imports: [
    CommonModule,
    CourseSummaryComponent,
    EvaluationStatusComponent,
    StudentDetailComponent,
    StudentListComponent,
    BmbTabsComponent,
    BmbLoaderComponent,
  ],
  templateUrl: './evaluation.component.html',
  styleUrl: './evaluation.component.css'
})
export class EvaluationComponent implements OnInit {
  private readonly stateService = inject(StateService);

  // Acceso a señales expuestas como solo lectura desde el servicio
  viewModel = this.stateService.viewModel;
  loading = this.stateService.loading;
  error = this.stateService.error;

  // Variables locales para simplificar el HTML
  course = computed(() => this.viewModel()?.course);
  courseState = computed(() => this.viewModel()?.courseState);
  students = computed(() => this.viewModel()?.students);
  selectedStudent = computed(() => this.viewModel()?.selectedStudent);
  evaluationStructures = computed(() => this.viewModel()?.evaluationStructures || []);

  tabs: IBmbTab[] = [
    { id: 1, title: 'Evaluar por alumnos', isActive: true },
    { id: 2, title: 'Evaluar por competencia/subcompetencia' },
  ];

  ngOnInit(): void {
    this.stateService.load();
  }

  onStudentSelected(studentId: string): void {
      this.stateService.selectStudent(studentId);
  }

  handleTabSelected(tab: IBmbTab): void {
    this.stateService.navigate(tab);
  }

}
