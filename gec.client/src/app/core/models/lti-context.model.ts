export interface Resolve {
  errorMessage: string;
  result: LtiContext;
  timeGenerated: Date;
}

export interface LtiContext {
  user: User;
  course: Course;
}

export interface User {
  name: string;
  email: string;
  userId: string;
  isInstructor: boolean;
  isStudent: boolean;
  isWithoutRole: boolean;
  picture: string;
}

export interface Course {
  id: string;
  label: string;
  title: string;
  type: string;
}
