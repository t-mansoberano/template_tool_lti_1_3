import {Component, Input} from '@angular/core';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-student-header',
  standalone: true,
  imports: [
    NgIf
  ],
  templateUrl: './student-header.component.html',
  styleUrl: './student-header.component.css'
})
export class StudentHeaderComponent {
  @Input() student: { id: string; name: string } | null = null;
}
