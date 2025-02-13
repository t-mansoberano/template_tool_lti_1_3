import {Component, Input, OnInit} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {
  BmbAccordionComponent,
  BmbBadgeComponent,
  BmbButtonDirective,
  BmbButtonGroupDirective,
  BmbCardComponent,
  BmbCardContentComponent,
  BmbCardHeaderComponent,
  BmbCheckboxComponent,
  BmbInputComponent,
  BmbRadialComponent
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {EvaluationStructureModel} from '../../models/evaluation-structure.model';
import {StudentEvaluationResultsModel} from '../../models/student-evaluation-results.model';
import {EvaluationResultModel} from '../../models/evaluation-result.model';
import {ReactiveFormsModule} from '@angular/forms';
import {DescriptorModel} from '../../models/descriptor.model';

@Component({
  selector: 'app-student-evaluation',
  standalone: true,
  imports: [
    NgForOf,
    BmbCardComponent,
    BmbCardHeaderComponent,
    BmbCardContentComponent,
    BmbAccordionComponent,
    BmbCheckboxComponent,
    BmbBadgeComponent,
    BmbInputComponent,
    BmbButtonDirective,
    BmbButtonGroupDirective,
    NgIf,
    ReactiveFormsModule,
    BmbRadialComponent,
  ],
  templateUrl: './student-evaluation.component.html',
  styleUrl: './student-evaluation.component.css'
})
export class StudentEvaluationComponent implements OnInit {
  @Input() evaluationResults!: StudentEvaluationResultsModel;
  @Input() evaluationStructures!: EvaluationStructureModel[];

  filteredEvaluations: EvaluationStructureModel[] = [];
  currentFilter: 'all' | 'evaluated' | 'pending' = 'all';

  // Array para mantener el estado de expansión de cada acordeón
  expandedStates: boolean[] = [];

  ngOnInit(): void {
    this.filteredEvaluations = this.evaluationStructures;
    this.initializeExpandedStates();
  }

  getEvaluationResult(evaluationId: string): EvaluationResultModel {
    return this.evaluationResults.evaluationResults.find(result => result.id === evaluationId) || {} as EvaluationResultModel;
  }

  filterEvaluations(filter: 'all' | 'evaluated' | 'pending') {
    this.currentFilter = filter;
    if (filter === 'all') {
      this.filteredEvaluations = this.evaluationStructures;
    } else {
      this.filteredEvaluations = this.evaluationStructures.filter(evaluation => {
        const evalResult = this.getEvaluationResult(evaluation.id);
        return filter === 'evaluated' ? evalResult.isEvaluated : !evalResult.isEvaluated;
      });
    }

    // Después de cambiar el filtro, asegúrate de que expandedStates se inicialice correctamente
    this.initializeExpandedStates();
  }

  // Método para inicializar expandedStates según el tamaño de filteredEvaluations
  initializeExpandedStates(): void {
    // Rellena expandedStates con 'false' para cada elemento en filteredEvaluations
    this.expandedStates = new Array(this.filteredEvaluations.length).fill(false);
  }

  // Método que se ejecuta cuando el acordeón es toggled
  onAccordionToggle(index: number): void {
    // Alterna el estado de expansión del acordeón
    this.expandedStates[index] = !this.expandedStates[index];
  }

  handleCheckboxChange(evaluationStructure: EvaluationStructureModel, descriptor: DescriptorModel) {
    console.log(evaluationStructure, descriptor);
  }
}
