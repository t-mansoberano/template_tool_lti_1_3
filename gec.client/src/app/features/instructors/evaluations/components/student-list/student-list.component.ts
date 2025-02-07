import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {NgForOf} from "@angular/common";
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
    search: new FormControl<string>('')
  });

  ngOnInit(): void {
    this.filterForm.get('search')?.valueChanges
      .subscribe(value => {
        this.filterText = value?.toLowerCase() || '';
        this.updateFilteredStudents();
      });

    // Primer cálculo
    this.updateFilteredStudents();
  }

  trackByStudentId(index: number, student: StudentModel): number | string {
    return student.id;
  }

  filteredStudents: StudentModel[] = [];

  selectStudent(student: StudentModel): void {
    this.studentSelected.emit(student);
  }

  handleTabSelected($event: IBmbTab) {
    this.activeTabId = $event.id;
    this.updateFilteredStudents();
  }

  private updateFilteredStudents(): void {
    const tabFilteredStudents = this.getTabFiltered(this.students);
    this.filteredStudents = this.getSearchFiltered(tabFilteredStudents, this.filterText);
  }

  private getTabFiltered(students: StudentModel[]): StudentModel[] {
    switch (this.activeTabId) {
      case 2: // 'Evaluados' = status = 'Evaluated'
        return students.filter(s => s.status === 'Completed');
      case 3: // 'Por evaluar' = status = 'Pending'
        return students.filter(s => s.status === 'Pending');
      default: // 'Todos'
        return students;
    }
  }

  private getSearchFiltered(students: StudentModel[], searchTerm: string): StudentModel[] {
    if (!searchTerm) return students;
    const term = searchTerm.toLowerCase();
    return students.filter(
      s => s.loginId.toLowerCase().includes(term) || s.name.toLowerCase().includes(term)
    );
  }

  protected readonly String = String;
}
