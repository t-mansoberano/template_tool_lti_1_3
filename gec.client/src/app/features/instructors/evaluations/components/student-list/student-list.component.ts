import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {NgForOf, NgIf} from "@angular/common";
import {
  BmbCardComponent,
  BmbCardContentComponent,
  BmbCardHeaderComponent,
  IBmbTab,
  BmbTabsComponent, BmbContainerComponent, BmbInputComponent, BmbLegendComponent, BmbBadgeComponent
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {BmbListGroupComponent, BmbListGroupItemComponent} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {StudentModel} from '../../models/student.model';
import {CourseStateModel} from '../../models/course-state.model';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    NgForOf,
    BmbCardComponent,
    BmbTabsComponent,
    BmbCardHeaderComponent,
    BmbCardContentComponent,
    BmbContainerComponent,
    BmbListGroupComponent,
    BmbListGroupItemComponent,
    BmbInputComponent,
    BmbBadgeComponent
  ],
  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css'
})
export class StudentListComponent implements OnInit {
  @Input() students!: StudentModel[];
  @Output() studentSelected = new EventEmitter<StudentModel>();
  @Input() courseState!: CourseStateModel;
  tabsData: IBmbTab[] = [
    {id: 1, title: 'Todos', isActive: true},
    {id: 2, title: 'Evaluados'},
    {id: 3, title: 'Por evaluar'},
  ];
  activeTabId: number = 1;
  filterText: string = '';
  filterForm = new FormGroup({
    search: new FormControl('')
  });

  ngOnInit(): void {
    this.filterForm.get('search')?.valueChanges.subscribe(value => {
      console.log('Filtro actualizado:', value);
      this.filterText = value?.toLowerCase() || '';
    });
  }

  // Getter para aplicar ambos filtros: por pestaña y por búsqueda
  get filteredStudents(): StudentModel[] {
    // Filtrar primero según la pestaña seleccionada
    let tabFilteredStudents: StudentModel[];

    switch (this.activeTabId) {
      case 2: // Evaluados: evaluaciones pendientes igual a 0
        tabFilteredStudents = this.students.filter(student => student.status === 'Completed');
        break;
      case 3: // Por evaluar: evaluaciones pendientes mayor que 0
        tabFilteredStudents = this.students.filter(student => student.status == 'Pending');
        break;
      default: // Todos
        tabFilteredStudents = this.students;
        break;
    }

    // Aplicar el filtro de búsqueda (loginId y name)
    const searchTerm = this.filterText.toLowerCase();
    if (!searchTerm) {
      return tabFilteredStudents;
    }

    return tabFilteredStudents.filter(student =>
      student.loginId.toLowerCase().includes(searchTerm) ||
      student.name.toLowerCase().includes(searchTerm)
    );
  }

  selectStudent(student: StudentModel): void {
    this.studentSelected.emit(student);
  }

  handleTabSelected($event: IBmbTab) {
    this.activeTabId = $event.id;
  }

  protected readonly String = String;
}
