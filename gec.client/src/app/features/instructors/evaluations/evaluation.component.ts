import {CommonModule, NgForOf, NgIf} from '@angular/common';
import {Component, inject} from '@angular/core';
import {TabsComponent} from './components/tabs/tabs.component';
import {InstructorHeaderComponent} from './components/instructor-header/instructor-header.component';
import {StudentListComponent} from './components/student-list/student-list.component';
import {StudentHeaderComponent} from './components/student-header/student-header.component';
import {FeedbackCardComponent} from './components/feedback-card/feedback-card.component';
import {CompetencyCardComponent} from './components/competency-card/competency-card.component';
import {EvaluationDataService} from './services/evaluation-data.service';
import {Evaluation} from './models/evaluation.model';

@Component({
  selector: 'app-evaluation',
  standalone: true,
  imports: [
    CommonModule,
    TabsComponent,
    InstructorHeaderComponent,
    StudentListComponent,
    StudentHeaderComponent,
    FeedbackCardComponent,
    CompetencyCardComponent,
    NgIf,
    NgForOf,
  ],
  templateUrl: './evaluation.component.html',
  styleUrl: './evaluation.component.css'
})
export class EvaluationComponent {
  private evaluationDataService = inject(EvaluationDataService);
  evaluation: Evaluation | null = null;
  canvasData: any = null;

  tabs = ['Evaluar por alumnos', 'Evaluar por competencia/subcompetencia'];
  activeTab = this.tabs[0];
  selectedStudent: any = {};

  ngOnInit(): void {
    this.loadEvaluation();
    this.loadTestCanvasAPI();
  }

  loadEvaluation(): void {
    this.evaluationDataService.getEvaluations().subscribe({
      next: (data) => {
        this.evaluation = data;
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {
        console.log('complete');
      }
    });
  }

  loadTestCanvasAPI(): void {
    this.evaluationDataService.getTestCanvasAPI().subscribe({
      next: (data) => {
        this.canvasData = data;
        console.log(data);
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {
        console.log('complete');
      }
    });
  }

  onTabChange(tab: string) {
    this.activeTab = tab;
  }

  onStudentSelected(studentId: string) {
    this.selectedStudent = this.evaluation?.students.find((s: { id: string; }) => s.id === studentId);
  }

  onCompetencyEvaluated(level: string) {
    console.log(`Competency evaluated as: ${level}`);
  }

}
