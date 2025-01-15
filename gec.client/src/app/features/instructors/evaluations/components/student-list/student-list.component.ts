import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgForOf} from "@angular/common";

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
  @Input() students!: { id: string; name: string; status: string }[];
  @Output() studentSelected = new EventEmitter<string>();

  selectStudent(studentId: string): void {
    this.studentSelected.emit(studentId);
  }
}
