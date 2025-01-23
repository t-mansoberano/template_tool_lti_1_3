import {CommonModule} from '@angular/common';
import {Component, inject, OnInit} from '@angular/core';
import {EvaluationDataService} from './services/evaluation-data.service';
import {CourseSummaryComponent} from './components/course-summary/course-summary.component';
import {EvaluationStatusComponent} from './components/evaluation-status/evaluation-status.component';
import {StudentListComponent} from './components/student-list/student-list.component';
import {StudentDetailComponent} from './components/student-detail/student-detail.component';
import { BmbTabsComponent, IBmbTab } from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {ActivatedRoute, Router} from '@angular/router';
import {ViewModel} from './models/view.model';

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
  ],
  templateUrl: './evaluation.component.html',
  styleUrl: './evaluation.component.css'
})
export class EvaluationComponent implements OnInit {
  route = inject(ActivatedRoute);
  router = inject(Router);
  viewModel: ViewModel | null = null;
  loading = true; // Indicador de carga
  tabs : IBmbTab[] = [{id: 1, title: 'Evaluar por alumnos', isActive: true}, {id: 2, title: 'Evaluar por competencia/subcompetencia'}]
  private readonly evaluationDataService = inject(EvaluationDataService);

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.evaluationDataService.getTestEvaluations().subscribe({
      next: (viewModel) => {
        this.viewModel = viewModel;
        this.tabs[0].badge = this.viewModel?.courseState?.totalStudents;
        this.tabs[1].badge = this.viewModel?.courseState?.totalStudents;
        console.log(this.viewModel);
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {
        this.loading = false; // Datos cargados
        console.log('complete');
      }
    })
  }

  onStudentSelected(studentId: string): void {
    if (this.viewModel) {
      this.viewModel.selectedStudent = this.viewModel.students.find(student => student.id === studentId) || null;
    }
  }

  handleTabSelected($event: IBmbTab) {
    this.router.navigate(['instructor-competencies'], {relativeTo: this.route.parent});
    console.log($event);
  }
}
