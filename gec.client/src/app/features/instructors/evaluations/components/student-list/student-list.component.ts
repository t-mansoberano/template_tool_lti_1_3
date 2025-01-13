import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-student-list',
  standalone: true,
  imports: [
    NgForOf
  ],
  templateUrl: './student-list.component.html',
  styleUrl: './student-list.component.css'
})
export class StudentListComponent {
  @Input() students: { id: string; name: string; evaluated: number; total: number }[] = [];
  @Output() studentSelected = new EventEmitter<string>();

  selectStudent(studentId: string) {
    this.studentSelected.emit(studentId);
  }
}
