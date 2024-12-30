import {Component, Input} from '@angular/core';

@Component({
  selector: 'app-rubric-card',
  standalone: true,
  imports: [],
  templateUrl: './rubric-card.component.html',
  styleUrl: './rubric-card.component.css'
})
export class RubricCardComponent {
  @Input() rubric: { title: string; description: string } | null = null;
}
