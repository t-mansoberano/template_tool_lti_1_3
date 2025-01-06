import {Context} from './context.model';

export interface Resolve {
  errorMessage: string;
  result: Context;
  timeGenerated: Date;
}
