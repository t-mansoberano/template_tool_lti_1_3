import {Component, Input, Output, EventEmitter, signal, computed, OnInit} from '@angular/core';
import {ReactiveFormsModule, FormGroup, FormControl} from '@angular/forms';
import {NgForOf} from '@angular/common';
import {
  BmbCardComponent,
  BmbTabsComponent,
  BmbCardHeaderComponent,
  BmbCardContentComponent,
  BmbContainerComponent,
  BmbListGroupComponent,
  BmbListGroupItemComponent,
  BmbInputComponent,
  BmbBadgeComponent,
  IBmbTab, BmbLayoutDirective, BmbLayoutItemDirective,
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';

import {StudentModel} from '../../models/student.model';
import {CourseStateModel} from '../../models/course-state.model';

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
    BmbBadgeComponent,
    BmbLayoutDirective,
    BmbLayoutItemDirective
  ],
  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css'
})
export class StudentListComponent implements OnInit {
  @Input() selectedStudentId?: number;
  /**
   * 1) Convertimos `students` en un setter de @Input para asignar a un Signal interno.
   *    De esta forma, cada vez que se reciba un nuevo array de estudiantes, actualizamos el Signal.
   */
  private _students = signal<StudentModel[]>([]);
  @Input() set students(value: StudentModel[]) {
    this._students.set(value ?? []);
  }

  /**
   * 2) También podemos hacer lo mismo con `courseState` si deseamos,
   *    pero aquí lo mantenemos como un Input normal.
   */
  @Input() courseState!: CourseStateModel;

  // Emite el estudiante seleccionado
  @Output() studentSelected = new EventEmitter<StudentModel>();

  tabsData: IBmbTab[] = [
    {id: 1, title: 'Todos', isActive: true},
    {id: 2, title: 'Evaluados'},
    {id: 3, title: 'Por evaluar'},
  ];
  // 3) Definimos un signal para la pestaña activa
  activeTabId = signal<number>(1);

  // 4) Definimos un signal para el texto de búsqueda
  searchSignal = signal<string>('');

  // 5) Formulario reactivo (opcional, lo combinamos con el signal)
  filterForm = new FormGroup({
    search: new FormControl<string>('', {nonNullable: true})
  });

  /**
   * 6) Creamos un "computed" que dependa de:
   *    - la lista de estudiantes (this._students())
   *    - la pestaña activa (this.activeTabId())
   *    - el texto de búsqueda (this.searchSignal())
   */
  filteredStudents = computed<StudentModel[]>(() => {
    const students = this._students();
    const tabId = this.activeTabId();
    const search = this.searchSignal().toLowerCase().trim();

    // Filtra según la pestaña
    const tabFiltered = this.filterByTab(students, tabId);

    // Aplica el filtro de búsqueda
    return this.filterBySearch(tabFiltered, search);
  });

  /**
   * 7) Efecto para “sincronizar” el valor del FormControl con el Signal
   *    Cada vez que cambie el control de búsqueda, actualizamos searchSignal, para
   *     “reaccionar” al valor del form en cada cambio.)
   */
  ngOnInit(): void {
    // Ojo: Los form controls no son "Signals" nativos, así que seguimos usando subscribe()
    this.filterForm.get('search')?.valueChanges.subscribe(value => {
      this.searchSignal.set(value.toLowerCase().trim());
    });
  }

  trackByStudentId(index: number, student: StudentModel): number | string {
    return student.id;
  }

  /**
   * Filtra estudiantes según la pestaña activa
   */
  private filterByTab(students: StudentModel[], activeTabId: number): StudentModel[] {
    switch (activeTabId) {
      case 2: // 'Evaluados'
        return students.filter(s => s.status === 'Completed');
      case 3: // 'Por evaluar'
        return students.filter(s => s.status === 'Pending');
      default: // 'Todos'
        return students;
    }
  }

  /**
   * Aplica el filtro de búsqueda
   */
  private filterBySearch(students: StudentModel[], searchTerm: string): StudentModel[] {
    if (!searchTerm) return students;
    return students.filter(
      s => s.loginId.toLowerCase().includes(searchTerm) ||
        s.name.toLowerCase().includes(searchTerm)
    );
  }

  /**
   * Se llama desde el template al hacer clic en un alumno
   */
  selectStudent(student: StudentModel): void {
    this.studentSelected.emit(student);
  }

  /**
   * Se llama cuando seleccionas una pestaña en <bmb-tabs>
   */
  handleTabSelected(tab: IBmbTab) {
    this.activeTabId.set(tab.id);
  }

  protected readonly String = String;
}
