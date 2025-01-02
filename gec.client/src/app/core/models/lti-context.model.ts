import {User} from './user.model';
import {Course} from './course.model';

export interface LtiContext {
  user: User;
  course: Course;
}

