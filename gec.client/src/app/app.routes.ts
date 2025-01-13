import {Routes} from '@angular/router';
import {ltiContextResolver} from './core/resolvers/lti-context.resolver';
import {ErrorPageComponent} from './core/components/error-page/error-page.component';
import {authGuard} from './core/guards/auth.guard';
import {ParentComponent} from './core/components/parent/parent.component';
import {LoadingComponent} from './core/components/loading/loading.component';
import {EvaluationComponent as Intructor} from './features/instructors/evaluations/evaluation.component';
import {EvaluationComponent as Student} from './features/students/evaluations/evaluation.component';
import {
  EvaluationComponent as ExternelCollaborator
} from './features/external-collaborators/evaluations/evaluation.component';

export const appRoutes: Routes = [
  {
    path: '',
    component: ParentComponent,
    resolve: {context: ltiContextResolver},
    children: [
      {path: '', redirectTo: '/loading-view', pathMatch: 'full'},
      {
        path: 'loading-view',
        component: LoadingComponent
      },
      {
        path: 'instructor',
        component: Intructor,
        canActivate: [authGuard],
      },
      {
        path: 'student',
        component: Student,
        canActivate: [authGuard],
      },
      {
        path: 'external-collaborator',
        component: ExternelCollaborator,
        canActivate: [authGuard]
      }
    ],
  },
  {path: 'error', component: ErrorPageComponent},
  {path: '**', redirectTo: '/**'},
];
