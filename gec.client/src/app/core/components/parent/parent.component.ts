import { Component, inject } from '@angular/core';
import { RouterOutlet, ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-parent',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './parent.component.html',
  styleUrl: './parent.component.css'
})
export class ParentComponent {
  route = inject(ActivatedRoute);
  router = inject(Router);
  constructor() {
    // Accede al contexto resuelto
    this.route.data.subscribe((data) => {
      const context = data['context'];
      if (context) {
        // Redirige basado en roles
        if (context.result.user.isInstructor) {
          this.router.navigate(['professor-view'], { relativeTo: this.route });
        } else if (context.result.user.isStudent) {
          this.router.navigate(['student-view'], { relativeTo: this.route });
        } else {
          this.router.navigate(['/error']);
        }
      } else {
        this.router.navigate(['/error']);
      }
    });
  }
}
