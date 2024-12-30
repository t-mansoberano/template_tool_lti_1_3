import {Evaluation} from './evaluation.model';

export interface Student {
  id: string;
  name: string;
  evaluations: Evaluation[];
}
