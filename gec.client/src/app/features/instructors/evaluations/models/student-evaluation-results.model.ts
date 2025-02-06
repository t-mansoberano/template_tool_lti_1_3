import {EvidenceModel} from './evidence.model';
import {EvaluationResultModel} from './evaluation-result.model';

export interface StudentEvaluationResultsModel {
  studentId: string;
  evaluationResults: EvaluationResultModel[];
}
