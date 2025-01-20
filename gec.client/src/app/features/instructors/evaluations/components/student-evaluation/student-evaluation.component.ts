import {Component, Input} from '@angular/core';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-student-evaluation',
  standalone: true,
  imports: [
    NgForOf,
  ],
  templateUrl: './student-evaluation.component.html',
  styleUrl: './student-evaluation.component.css'
})
export class StudentEvaluationComponent {
  @Input() evaluationResults!: { id: string; achievementLevel: string; comments: string }[];
  @Input() evaluationStructures!: { id: string; name: string }[];

  getEvaluationName(evaluationId: string): string {
    const evaluation = this.evaluationStructures.find(e => e.id === evaluationId);
    return evaluation ? evaluation.name : 'Unknown';
  }
}
