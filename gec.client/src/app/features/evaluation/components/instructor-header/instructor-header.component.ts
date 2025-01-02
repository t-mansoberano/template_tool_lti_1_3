import {Component, Input} from '@angular/core';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-professor-header',
  standalone: true,
  imports: [
    NgIf
  ],
  templateUrl: './instructor-header.component.html',
  styleUrl: './instructor-header.component.css'
})
export class InstructorHeaderComponent {
  @Input() title: string = '';
  @Input() subtitle: string = '';
}
