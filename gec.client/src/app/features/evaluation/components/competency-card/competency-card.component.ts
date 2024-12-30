import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-competency-card',
  standalone: true,
  imports: [
    NgIf
  ],
  templateUrl: './competency-card.component.html',
  styleUrl: './competency-card.component.css'
})
export class CompetencyCardComponent {
  @Input() competency: { name: string; description: string } | null = null;
  @Output() competencyEvaluated = new EventEmitter<string>();

  evaluateCompetency(level: string) {
    this.competencyEvaluated.emit(level);
  }
}
