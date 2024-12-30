import {Component, Input} from '@angular/core';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-professor-header',
  standalone: true,
  imports: [
    NgIf
  ],
  templateUrl: './professor-header.component.html',
  styleUrl: './professor-header.component.css'
})
export class ProfessorHeaderComponent {
  @Input() title: string = '';
  @Input() subtitle: string = '';
}
