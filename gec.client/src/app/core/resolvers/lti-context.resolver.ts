import {ResolveFn} from '@angular/router';
import {inject} from '@angular/core';
import {AuthService} from '../services/auth.service';
import {Resolve} from '../models/resolve.model';

export const ltiContextResolver: ResolveFn<Resolve> = (route, state) => {
  const ltiService = inject(AuthService);
  return ltiService.getLtiContext();
};
