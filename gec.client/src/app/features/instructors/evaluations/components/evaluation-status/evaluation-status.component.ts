import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-evaluation-status',
  standalone: true,
  imports: [],
  templateUrl: './evaluation-status.component.html',
  styleUrl: './evaluation-status.component.css'
})
export class EvaluationStatusComponent {
  @Input() courseState!: { totalStudents: number; evaluatedStudents: number; pendingStudents: number; evaluationStatus: string };
}
