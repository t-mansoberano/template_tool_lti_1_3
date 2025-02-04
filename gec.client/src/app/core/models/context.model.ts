import {User} from './user.model';
import {Course} from './course.model';

export interface Context {
  user: User;
  course: Course;
}

