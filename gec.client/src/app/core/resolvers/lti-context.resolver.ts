import {ResolveFn} from '@angular/router';
import {inject} from '@angular/core';
import {AuthService} from '../services/auth.service';
import {Context} from '../models/context.model';

export const ltiContextResolver: ResolveFn<Context> = (route, state) => {
  const ltiService = inject(AuthService);
  return ltiService.getLtiContext();
};
