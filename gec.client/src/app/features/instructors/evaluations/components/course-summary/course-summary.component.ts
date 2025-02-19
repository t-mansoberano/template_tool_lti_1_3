import {Component, Input} from '@angular/core';
import {
  BmbDividerComponent, BmbIconComponent, BmbLayoutDirective, BmbLayoutItemDirective
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {CourseModel} from '../../models/course.model';

@Component({
  selector: 'app-course-summary',
  standalone: true,
  imports: [BmbDividerComponent, BmbIconComponent, BmbLayoutDirective, BmbLayoutItemDirective],
  templateUrl: './course-summary.component.html',
  styleUrl: './course-summary.component.css'
})
export class CourseSummaryComponent {
  @Input() course!: CourseModel;
  protected readonly String = String;
}
