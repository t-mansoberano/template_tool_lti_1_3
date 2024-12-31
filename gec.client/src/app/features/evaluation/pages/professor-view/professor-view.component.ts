import {CommonModule} from '@angular/common'; // Importar CommonModule
import {Component, inject} from '@angular/core';
import {Evaluation} from '../../models/evaluation.model';
import {EvaluationDataService} from '../../services/evaluation-data.service';
import {TabsComponent} from '../../components/tabs/tabs.component';
import {ProfessorHeaderComponent} from '../../components/professor-header/professor-header.component';
import {StudentListComponent} from '../../components/student-list/student-list.component';
import {StudentHeaderComponent} from '../../components/student-header/student-header.component';
import {FeedbackCardComponent} from '../../components/feedback-card/feedback-card.component';
import {CompetencyCardComponent} from '../../components/competency-card/competency-card.component';
import {NgForOf, NgIf} from '@angular/common';
import {HeaderComponent} from '../../../../shared/components/header/header.component';

@Component({
  selector: 'app-professor-view',
  standalone: true,
  imports: [
    CommonModule,
    TabsComponent,
    ProfessorHeaderComponent,
    StudentListComponent,
    StudentHeaderComponent,
    FeedbackCardComponent,
    CompetencyCardComponent,
    NgIf,
    NgForOf,
    HeaderComponent,
  ],
  templateUrl: './professor-view.component.html',
  styleUrl: './professor-view.component.css'
})
export class ProfessorViewComponent {
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
