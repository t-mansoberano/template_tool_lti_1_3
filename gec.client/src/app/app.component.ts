import {Component} from '@angular/core';
import {RouterOutlet} from '@angular/router';

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
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
}
