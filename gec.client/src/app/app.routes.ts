import { Routes } from '@angular/router';
import { StudentViewComponent } from './features/evaluation/pages/student-view/student-view.component';
import { ProfessorViewComponent } from './features/evaluation/pages/professor-view/professor-view.component';

export const appRoutes: Routes = [
  { path: '', redirectTo: '/professor-view', pathMatch: 'full' },
  { path: 'professor-view', component: ProfessorViewComponent },
  { path: 'student-view', component: StudentViewComponent },
  { path: '**', redirectTo: '/professor-view' },
];
