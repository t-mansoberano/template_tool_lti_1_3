import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-course-summary',
  standalone: true,
  imports: [],
  templateUrl: './course-summary.component.html',
  styleUrl: './course-summary.component.css'
})
export class CourseSummaryComponent {
  @Input() course!: { id: string; key: string; name: string };
}
