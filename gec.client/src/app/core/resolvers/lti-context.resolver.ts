import {ResolveFn} from '@angular/router';
import {Resolve} from '../models/lti-context.model';
import {inject} from '@angular/core';
import {AuthService} from '../services/auth.service';

export const ltiContextResolver: ResolveFn<Resolve> = (route, state) => {
  const ltiService = inject(AuthService);
  return ltiService.getLtiContext();
};
