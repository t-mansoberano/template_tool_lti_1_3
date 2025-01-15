import {CommonModule} from '@angular/common';
import {Component, inject, OnInit} from '@angular/core';
import {EvaluationDataService} from './services/evaluation-data.service';
import {Evaluation} from './models/evaluation.model';
import {CourseSummaryComponent} from './components/course-summary/course-summary.component';
import {EvaluationStatusComponent} from './components/evaluation-status/evaluation-status.component';
import {StudentListComponent} from './components/student-list/student-list.component';
import {StudentDetailComponent} from './components/student-detail/student-detail.component';

@Component({
  selector: 'app-evaluation',
  standalone: true,
  imports: [
    CommonModule,
    CourseSummaryComponent,
    EvaluationStatusComponent,
    StudentDetailComponent,
    StudentListComponent
  ],
  templateUrl: './evaluation.component.html',
  styleUrl: './evaluation.component.css'
})
export class EvaluationComponent implements OnInit {
  evaluation: Evaluation | null = null;
  loading = true; // Indicador de carga
  private readonly evaluationDataService = inject(EvaluationDataService);

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.evaluationDataService.getTestEvaluations().subscribe({
      next: (data) => {
        this.evaluation = data.result;
        console.log(this.evaluation);
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
    if (this.evaluation) {
      this.evaluation.selectedStudent = this.evaluation.students.find(student => student.id === studentId) || null;
    }
  }

}
