import {Component, Input} from '@angular/core';
import { BmbDividerComponent, BmbIconComponent } from '@ti-tecnologico-de-monterrey-oficial/ds-ng';

@Component({
  selector: 'app-course-summary',
  standalone: true,
  imports: [BmbDividerComponent, BmbIconComponent],
  templateUrl: './course-summary.component.html',
  styleUrl: './course-summary.component.css'
})
export class CourseSummaryComponent {
  @Input() course!: { id: string; key: string; name: string };
}
