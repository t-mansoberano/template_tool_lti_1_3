import {Component, EventEmitter, Input, Output} from '@angular/core';
import {NgForOf} from '@angular/common';

@Component({
  selector: 'app-tabs',
  standalone: true,
  imports: [
    NgForOf
  ],
  templateUrl: './tabs.component.html',
  styleUrl: './tabs.component.css'
})
export class TabsComponent {
  @Input() tabs: string[] = [];
  @Input() activeTab: string = '';
  @Output() tabSelected = new EventEmitter<string>();

  selectTab(tab: string) {
    this.tabSelected.emit(tab);
  }
}
