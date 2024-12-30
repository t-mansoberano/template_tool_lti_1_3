import {Component, Input} from '@angular/core';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-feedback-card',
  standalone: true,
  imports: [
    NgIf
  ],
  templateUrl: './feedback-card.component.html',
  styleUrl: './feedback-card.component.css'
})
export class FeedbackCardComponent {
  @Input() feedback: { title: string; description: string; score: string } | null = null;
}
