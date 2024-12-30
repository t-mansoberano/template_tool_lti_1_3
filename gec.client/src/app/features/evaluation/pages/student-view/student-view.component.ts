import {Component} from '@angular/core';
import {Student} from '../../models/student.model';
import {StudentViewService} from '../../services/student-view.service';

@Component({
  selector: 'app-student-view',
  standalone: true,
  imports: [],
  templateUrl: './student-view.component.html',
  styleUrl: './student-view.component.css'
})
export class StudentViewComponent {
  students: Student[] = [];

  constructor(private studentService: StudentViewService) {}

  ngOnInit(): void {
    this.studentService.getStudents().subscribe((data) => {
      this.students = data;
    });
  }
}
