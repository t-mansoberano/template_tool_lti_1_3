import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {
  BmbCardComponent,
  BmbCardContentComponent,
  BmbCardHeaderComponent,
  IBmbTab,
  BmbTabsComponent, BmbContainerComponent, BmbInputComponent, BmbLegendComponent, BmbBadgeComponent
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import { BmbListGroupComponent, BmbListGroupItemComponent } from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {StudentModel} from '../../models/student.model';
import {CourseStateModel} from '../../models/course-state.model';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [
    NgForOf,
    BmbCardComponent,
    BmbTabsComponent,
    BmbCardHeaderComponent,
    BmbCardContentComponent,
    BmbContainerComponent,
    NgIf,
    BmbListGroupComponent,
    BmbListGroupItemComponent,
    BmbInputComponent,
    BmbBadgeComponent
  ],
  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css'
})
export class StudentListComponent {
  @Input() students!: StudentModel[];
  @Output() studentSelected = new EventEmitter<string>();
  @Input() courseState!: CourseStateModel;
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

  protected readonly String = String;
}
