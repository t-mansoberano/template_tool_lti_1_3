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
  BmbCheckboxComponent, BmbIconComponent,
  BmbInputComponent, BmbLayoutDirective, BmbLayoutItemDirective, BmbListGroupComponent, BmbListGroupItemComponent,
  BmbRadialComponent, IBbmBgAppearance
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {EvaluationStructureModel} from '../../models/evaluation-structure.model';
import {StudentEvaluationResultsModel} from '../../models/student-evaluation-results.model';
import {EvaluationResultModel} from '../../models/evaluation-result.model';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
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
    BmbListGroupComponent,
    BmbListGroupItemComponent,
    BmbIconComponent,
    BmbLayoutDirective,
    BmbLayoutItemDirective,
  ],
  templateUrl: './student-evaluation.component.html',
  styleUrl: './student-evaluation.component.css'
})
export class StudentEvaluationComponent implements OnInit {
  @Input() evaluationResults!: StudentEvaluationResultsModel;
  @Input() evaluationStructures!: EvaluationStructureModel[];

  filteredEvaluations: EvaluationStructureModel[] = [];
  currentFilter: 'all' | 'evaluated' | 'pending' = 'all';

  public commentControls: { [evaluationId: string]: FormControl } = {};
  getOrCreateControl(evaluationId: string, initialValue: string): FormControl {
    // Si ya existe un control para esa evaluación, lo retornamos.
    if (this.commentControls[evaluationId]) {
      return this.commentControls[evaluationId];
    }
    // Si no, creamos uno nuevo y lo almacenamos en el diccionario.
    const newControl = new FormControl(initialValue, [Validators.required]);
    this.commentControls[evaluationId] = newControl;
    return newControl;
  }

  // Array para mantener el estado de expansión de cada acordeón
  expandedStates: boolean[] = [];

  ngOnInit(): void {
    this.filteredEvaluations = this.evaluationStructures;
    this.initializeExpandedStates();
  }

  getEvaluationResult(evaluationId: string): EvaluationResultModel {
    const result = this.evaluationResults.evaluationResults.find(result => result.id === evaluationId) || {} as EvaluationResultModel;
    return result;
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

  trackByEvaluationId(index: number, evaluation: EvaluationStructureModel): string {
    return evaluation.id;
  }

  getBadgeAppearance(level: 'Destacado' | 'Sólido' | 'Básico' | 'Incipiente' | 'NoElementosSuficientes'): IBbmBgAppearance  {
    const levels: { [key in 'Destacado' | 'Sólido' | 'Básico' | 'Incipiente' | 'NoElementosSuficientes']: IBbmBgAppearance } = {
      'Destacado': 'success',
      'Sólido': 'mitec_green',
      'Básico': 'mitec_light_green',
      'Incipiente': 'creative_licorice',
      'NoElementosSuficientes': 'normal',
    };
    return levels[level] || 'normal';
  }

  handleCheckboxChange($event: any, evaluationStructure: EvaluationStructureModel, descriptor: DescriptorModel) {
    const evalResult = this.getEvaluationResult(evaluationStructure.id);

    // Si el evento proviene del checkbox (descriptor.level === 'NoElementosSuficientes')
    if (descriptor.level === 'NoElementosSuficientes') {
      // Si actualmente se tenía seleccionado algún list-group-item (es decir, achievementLevel no es "NoElementosSuficientes" ni vacío)
      if ($event.currentTarget.checked) {
        // Se deselecciona el list-group-item y se activa el checkbox
        evalResult.achievementLevel = 'NoElementosSuficientes';
        evalResult.isEvaluated = true;
      } else {
        // Si ya estaba seleccionado el checkbox, se lo deselecciona (opción: asignar cadena vacía)
        evalResult.achievementLevel = '';
        evalResult.isEvaluated = false;
      }
    } else {
      // Si el evento proviene de un list-group-item
      // Si el checkbox estaba seleccionado, se deselecciona asignando el valor del item clickeado
      if (evalResult.achievementLevel === 'NoElementosSuficientes') {
        evalResult.achievementLevel = descriptor.level;
        evalResult.isEvaluated = true;
      } else if (evalResult.achievementLevel === descriptor.level) {
        // Si se hace clic sobre el mismo item activo, se puede alternar el estado (lo deselecciona)
        evalResult.achievementLevel = '';
        evalResult.isEvaluated = false;
      } else {
        // Si se hace clic en un item distinto, se asigna ese valor
        evalResult.achievementLevel = descriptor.level;
        evalResult.isEvaluated = true;
      }
    }
  }

  protected readonly String = String;
}
