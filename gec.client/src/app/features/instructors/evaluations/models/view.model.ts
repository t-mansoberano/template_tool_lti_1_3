import {StudentModel} from './student.model';
import {EvaluationStructureModel} from './evaluation-structure.model';
import {CourseStateModel} from './course-state.model';
import {CourseModel} from './course.model';

export interface ViewModel {
  course: CourseModel;
  courseState: CourseStateModel;
  students: StudentModel[];
  evaluationStructures: EvaluationStructureModel[];
  selectedStudent: StudentModel | null;
}

export interface ViewStudentModel {
  selectedStudent: StudentModel;
}
