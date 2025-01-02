export interface User {
  name: string;
  email: string;
  userId: string;
  isInstructor: boolean;
  isStudent: boolean;
  isExternalCollaborator: boolean;
  isWithoutRole: boolean;
  picture: string;
}
