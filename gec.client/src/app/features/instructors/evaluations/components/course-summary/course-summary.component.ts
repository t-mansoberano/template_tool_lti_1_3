import {Component, Input} from '@angular/core';
import { BmbDividerComponent, BmbIconComponent } from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {CourseModel} from '../../models/course.model';

@Component({
  selector: 'app-course-summary',
  standalone: true,
  imports: [BmbDividerComponent, BmbIconComponent],
  templateUrl: './course-summary.component.html',
  styleUrl: './course-summary.component.css'
})
export class CourseSummaryComponent {
  @Input() course!: CourseModel;
}
