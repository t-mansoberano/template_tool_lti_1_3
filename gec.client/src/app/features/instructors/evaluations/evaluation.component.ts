import {CommonModule} from '@angular/common';
import {Component, computed, inject, OnInit} from '@angular/core';
import {CourseSummaryComponent} from './components/course-summary/course-summary.component';
import {StudentListComponent} from './components/student-list/student-list.component';
import {StudentDetailComponent} from './components/student-detail/student-detail.component';
import {
  BmbBadgeComponent,
  BmbLoaderComponent,
  BmbTabsComponent,
  IBmbTab
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {StateService} from './services/state.service';
import {StudentModel} from './models/student.model';

@Component({
  selector: 'app-evaluation',
  standalone: true,
  imports: [
    CommonModule,
    CourseSummaryComponent,
    StudentDetailComponent,
    StudentListComponent,
    BmbTabsComponent,
    BmbLoaderComponent,
    BmbBadgeComponent,
  ],
  templateUrl: './evaluation.component.html',
  styleUrl: './evaluation.component.css'
})
export class EvaluationComponent implements OnInit {
  private readonly stateService = inject(StateService);

  // Acceso a señales expuestas como solo lectura desde el servicio
  readonly viewModel = this.stateService.viewModel;
  readonly loading = this.stateService.loading;
  readonly error = this.stateService.error;

  // Variables locales para simplificar el HTML
  readonly course = computed(() => this.viewModel()?.course || null);
  readonly courseState = computed(() => this.viewModel()?.courseState || null);
  readonly students = computed(() => this.viewModel()?.students || null);
  readonly selectedStudent = computed(() => this.viewModel()?.selectedStudent || null);
  readonly studentEvidences = computed(() => this.viewModel()?.studentEvidences || null);
  readonly studentEvaluationResults = computed(() => this.viewModel()?.studentEvaluationResults || null);
  readonly evaluationStructures = computed(() => this.viewModel()?.evaluationStructures || []);

  tabs = this.stateService.tabs;

  ngOnInit(): void {
    this.stateService.load();
  }

  onStudentSelected(student: StudentModel): void {
      this.stateService.selectStudent(student);
  }

  handleTabSelected(tab: IBmbTab): void {
    this.stateService.navigate(tab);
  }

}
