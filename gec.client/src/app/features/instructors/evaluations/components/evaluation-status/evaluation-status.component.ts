import {Component, Input} from '@angular/core';
import {BmbBadgeComponent} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';

@Component({
  selector: 'app-evaluation-status',
  standalone: true,
  imports: [
    BmbBadgeComponent
  ],
  templateUrl: './evaluation-status.component.html',
  styleUrl: './evaluation-status.component.css'
})
export class EvaluationStatusComponent {
  @Input() courseState!: { totalStudents: number; evaluatedStudents: number; pendingStudents: number; evaluationStatus: string };
}
