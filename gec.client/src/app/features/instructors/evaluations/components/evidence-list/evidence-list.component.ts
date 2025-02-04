import {Component, Input} from '@angular/core';
import {NgForOf, NgIf} from '@angular/common';
import {
  BmbButtonDirective,
  BmbCardComponent,
  BmbCardContentComponent, BmbCardFooterComponent, BmbIconComponent,
  BmbTextLinkComponent
} from '@ti-tecnologico-de-monterrey-oficial/ds-ng';
import {EvidenceModel} from '../../models/evidence.model';

@Component({
  selector: 'app-evidence-list',
  standalone: true,
  imports: [
    NgForOf,
    BmbCardComponent,
    BmbCardContentComponent,
    BmbCardFooterComponent,
    BmbButtonDirective,
    BmbTextLinkComponent,
    NgIf,
    BmbIconComponent
  ],
  templateUrl: './evidence-list.component.html',
  styleUrl: './evidence-list.component.css'
})
export class EvidenceListComponent {
  @Input() evidences!: EvidenceModel[];

  // Estado global para colapsar o expandir la lista
  isCollapsed: boolean = true;

  // Método para alternar entre colapsado y expandido
  toggleCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
  }
}
