import {FederationContext} from "./federation-context.model";
import {LtiContext} from './lti-context.model';

export interface Resolve {
  errorMessage: string;
  result: LtiContext | FederationContext;
  timeGenerated: Date;
}
