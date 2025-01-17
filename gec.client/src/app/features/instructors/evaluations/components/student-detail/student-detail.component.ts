import {Component, Input} from '@angular/core';
import {StudentEvaluationComponent} from '../student-evaluation/student-evaluation.component';
import {EvidenceListComponent} from '../evidence-list/evidence-list.component';
import {BmbDividerComponent} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';

@Component({
  selector: 'app-student-detail',
  standalone: true,
  imports: [
    StudentEvaluationComponent,
    EvidenceListComponent,
    BmbDividerComponent
  ],
  templateUrl: './student-detail.component.html',
  styleUrl: './student-detail.component.css'
})
export class StudentDetailComponent {
  @Input() student!: any;
  @Input() evaluationStructures!: any[];
}
