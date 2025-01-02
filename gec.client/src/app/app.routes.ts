import {Routes} from '@angular/router';
import {StudentViewComponent} from './features/evaluation/pages/student-view/student-view.component';
import {ltiContextResolver} from './core/resolvers/lti-context.resolver';
import {ErrorPageComponent} from './core/components/error-page/error-page.component';
import {authGuard} from './core/guards/auth.guard';
import {ParentComponent} from './core/components/parent/parent.component';
import {LoadingComponent} from './core/components/loading/loading.component';
import {InstructorViewComponent} from './features/evaluation/pages/instructor-view/instructor-view.component';
import {
  ExternalCollaboratorViewComponent
} from './features/evaluation/pages/external-collaborator-view/external-collaborator-view.component';

export const appRoutes: Routes = [
  {
    path: '',
    component: ParentComponent,
    resolve: { context: ltiContextResolver },
    children: [
      {path: '', redirectTo: '/loading-view', pathMatch: 'full'},
      {
        path: 'loading-view',
        component: LoadingComponent
      },
      {
        path: 'instructor-view',
        component: InstructorViewComponent,
        canActivate: [authGuard],
      },
      {
        path: 'student-view',
        component: StudentViewComponent,
        canActivate: [authGuard],
      },
      {
        path: 'external-collaborator-view',
        component: ExternalCollaboratorViewComponent,
        canActivate: [authGuard]
      }
    ],
  },
  {path: 'error', component: ErrorPageComponent},
  {path: '**', redirectTo: '/**'},
];
