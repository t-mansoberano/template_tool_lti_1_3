import {Component, Input, ViewEncapsulation} from '@angular/core';
import {NgClass, NgForOf, NgIf, NgStyle} from '@angular/common';
import {
  BmbButtonDirective,
  BmbCardComponent,
  BmbCardContentComponent, BmbCardFooterComponent,
  BmbCardHeaderComponent,
  BmbIconComponent, BmbTextLinkComponent
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
    NgIf,
    BmbCardFooterComponent,
    NgStyle,
    BmbButtonDirective,
    BmbTextLinkComponent
  ],
  templateUrl: './evidence-list.component.html',
  styleUrl: './evidence-list.component.css'
})
export class EvidenceListComponent {
  @Input() evidences!: { id: string; name: string; feedback: string; grade: number; speedGraderLink: string; fileLink: string; fileType: string }[];

  // Estado global para colapsar o expandir la lista
  isCollapsed: boolean = true;

  // Método para alternar entre colapsado y expandido
  toggleCollapse(): void {
    this.isCollapsed = !this.isCollapsed;
  }
}
