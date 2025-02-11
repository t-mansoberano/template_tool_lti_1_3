import {StudentModel} from './student.model';
import {EvaluationStructureModel} from './evaluation-structure.model';
import {CourseStateModel} from './course-state.model';
import {CourseModel} from './course.model';
import {StudentEvidencesModel} from './student-evidences.model';
import {StudentEvaluationResultsModel} from './student-evaluation-results.model';

export interface ViewModel {
  course: CourseModel;
  courseState: CourseStateModel;
  students: StudentModel[];
  studentEvidences: StudentEvidencesModel
  studentEvaluationResults: StudentEvaluationResultsModel
  evaluationStructures: EvaluationStructureModel[];
  selectedStudent: StudentModel;
}

export interface ViewStudentEvidencesModel {
  studentEvidences: StudentEvidencesModel;
  studentEvaluationResults: StudentEvaluationResultsModel
}
