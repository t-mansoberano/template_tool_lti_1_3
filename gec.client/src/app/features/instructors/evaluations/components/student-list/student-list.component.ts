import {Component, EventEmitter, Input, Output, ViewEncapsulation} from '@angular/core';
import {NgForOf, NgIf, NgStyle} from "@angular/common";
import {
  BmbCardComponent,
  BmbCardContentComponent,
  BmbCardFooterComponent,
  BmbCardHeaderComponent,
  IBmbTab,
  BmbTabsComponent, BmbContainerComponent, BmbInputComponent
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import { BmbListGroupComponent, BmbListGroupItemComponent } from '@ti-tecnologico-de-monterrey-oficial/ds-ng';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [
    NgForOf,
    BmbCardComponent,
    BmbTabsComponent,
    BmbCardHeaderComponent,
    BmbCardContentComponent,
    NgStyle,
    BmbContainerComponent,
    NgIf,
    BmbListGroupComponent,
    BmbListGroupItemComponent,
    BmbInputComponent
  ],
  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css',
  encapsulation: ViewEncapsulation.None, // Desactiva la encapsulación de estilos
})
export class StudentListComponent {
  @Input() students!: { id: string; name: string; status: string; completedEvaluations: number; pendingEvaluations: number; totalEvaluations: number }[];
  @Output() studentSelected = new EventEmitter<string>();
  @Input() courseState!: { totalStudents: number; evaluatedStudents: number; pendingStudents: number; evaluationStatus: string };
  tabsData: IBmbTab[] = [
    { id: 1, title: 'Todos', isActive: true },
    { id: 2, title: 'Evaluados' },
    { id: 3, title: 'Por evaluar' },
  ];
  activeTabId: number = 1;

  selectStudent(studentId: string): void {
    this.studentSelected.emit(studentId);
  }

  handleTabSelected($event: IBmbTab) {
    this.activeTabId = $event.id;
  }
}
