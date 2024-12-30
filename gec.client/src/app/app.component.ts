import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';
import {HeaderComponent} from './shared/components/header/header.component';

interface Response {
  errorMessage: string,
  timeGenerated: Date,
  result: any
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    HeaderComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
}
