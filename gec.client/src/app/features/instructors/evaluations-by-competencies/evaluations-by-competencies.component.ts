import { Component } from '@angular/core';
import { Location } from '@angular/common';

@Component({
  selector: 'app-evaluations-by-competencies',
  standalone: true,
  imports: [],
  templateUrl: './evaluations-by-competencies.component.html',
  styleUrl: './evaluations-by-competencies.component.css'
})
export class EvaluationsByCompetenciesComponent {
  constructor(private location: Location) {}

  goBack() {
      this.location.back();
  }

}
