import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Student } from '../models/student.model';

@Injectable({
  providedIn: 'root'
})
export class StudentViewService {
  getStudents(): Observable<Student[]> {
    return of([{
      id: '1',
      name: 'John Doe',
      evaluations: [],
    }]);
  }
}
