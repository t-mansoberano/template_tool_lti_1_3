import {Component, Input} from '@angular/core';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-evidence-list',
  standalone: true,
  imports: [
    NgForOf
  ],
  templateUrl: './evidence-list.component.html',
  styleUrl: './evidence-list.component.css'
})
export class EvidenceListComponent {
  @Input() evidences!: { id: string; name: string; feedback: string; grade: number; speedGraderLink: string }[];
}
