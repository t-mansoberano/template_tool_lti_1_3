import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {
  BmbAccordionComponent, BmbBadgeComponent, BmbButtonDirective, BmbButtonGroupDirective,
  BmbCardComponent,
  BmbCardContentComponent, BmbCardFooterComponent,
  BmbCardHeaderComponent, BmbCheckboxComponent, BmbHitoCardComponent, BmbIconComponent, BmbInputComponent
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {EvaluationStructureModel} from '../../models/evaluation-structure.model';
import {StudentEvaluationResultsModel} from '../../models/student-evaluation-results.model';
import {EvaluationResultModel} from '../../models/evaluation-result.model';

@Component({
  selector: 'app-student-evaluation',
  standalone: true,
  imports: [
    NgForOf,
    BmbCardComponent,
    BmbCardHeaderComponent,
    BmbCardContentComponent,
    BmbCardFooterComponent,
    BmbAccordionComponent,
    BmbCheckboxComponent,
    BmbHitoCardComponent,
    BmbBadgeComponent,
    BmbInputComponent,
    BmbIconComponent,
    BmbButtonDirective,
    BmbButtonGroupDirective,
    NgIf,
  ],
  templateUrl: './student-evaluation.component.html',
  styleUrl: './student-evaluation.component.css'
})
export class StudentEvaluationComponent {
  @Input() evaluationResults!: StudentEvaluationResultsModel;
  @Input() evaluationStructures!: EvaluationStructureModel[];

  getEvaluationName(evaluationId: string): string {
    const evaluation = this.evaluationStructures.find(e => e.id === evaluationId);
    return evaluation ? evaluation.name : 'Unknown';
  }

  getEvaluationResult(evaluationId: string) : EvaluationResultModel {
    return this.evaluationResults.evaluationResults.find(result => result.id === evaluationId) || {} as EvaluationResultModel;
  }

}
