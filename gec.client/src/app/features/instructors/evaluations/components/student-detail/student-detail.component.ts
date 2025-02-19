import {Component, Input} from '@angular/core';
import {StudentEvaluationComponent} from '../student-evaluation/student-evaluation.component';
import {EvidenceListComponent} from '../evidence-list/evidence-list.component';
import {BmbDividerComponent, BmbLayoutItemDirective} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {StudentModel} from '../../models/student.model';
import {EvaluationStructureModel} from '../../models/evaluation-structure.model';
import {StudentEvidencesModel} from '../../models/student-evidences.model';
import {StudentEvaluationResultsModel} from '../../models/student-evaluation-results.model';

@Component({
  selector: 'app-student-detail',
  standalone: true,
  imports: [
    StudentEvaluationComponent,
    EvidenceListComponent,
    BmbDividerComponent,
    BmbLayoutItemDirective
  ],
  templateUrl: './student-detail.component.html',
  styleUrl: './student-detail.component.css'
})
export class StudentDetailComponent {
  @Input() student!: StudentModel;
  @Input() studentEvidences!: StudentEvidencesModel;
  @Input() studentEvaluationResults!: StudentEvaluationResultsModel;
  @Input() evaluationStructures!: EvaluationStructureModel[];
}
