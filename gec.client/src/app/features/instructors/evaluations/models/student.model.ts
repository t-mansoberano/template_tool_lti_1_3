import {EvidenceModel} from './evidence.model';
import {EvaluationResultModel} from './evaluation-result.model';

export interface StudentModel {
  id: string;
  loginId: string;
  name: string;
  status: string; // Ejemplo: "Evaluated", "Pending", etc.
  totalEvaluations: number;
  completedEvaluations: number;
  pendingEvaluations: number;
  evidences: EvidenceModel[];
  evaluationResults: EvaluationResultModel[];
}
