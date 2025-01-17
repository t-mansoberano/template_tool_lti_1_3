import {Component, Input, ViewEncapsulation} from '@angular/core';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {
  BmbCardComponent,
  BmbCardContentComponent,
  BmbCardHeaderComponent,
  BmbIconComponent
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';

@Component({
  selector: 'app-evidence-list',
  standalone: true,
  imports: [
    NgForOf,
    BmbCardComponent,
    BmbCardContentComponent,
    BmbCardHeaderComponent,
    BmbIconComponent,
    NgClass,
    NgIf
  ],
  templateUrl: './evidence-list.component.html',
  styleUrl: './evidence-list.component.css'
})
export class EvidenceListComponent {
  @Input() evidences!: { id: string; name: string; feedback: string; grade: number; speedGraderLink: string; fileLink: string; fileType: string }[];
}
