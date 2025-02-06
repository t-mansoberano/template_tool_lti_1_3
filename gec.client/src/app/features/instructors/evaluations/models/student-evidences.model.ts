import {EvidenceModel} from './evidence.model';
import {EvaluationResultModel} from './evaluation-result.model';

export interface StudentEvidencesModel {
  studentId: string;
  evidences: EvidenceModel[];
}
