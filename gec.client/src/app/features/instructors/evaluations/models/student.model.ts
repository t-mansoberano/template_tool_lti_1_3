export interface StudentModel {
  id: string;
  loginId: string;
  name: string;
  status: string; // Ejemplo: "Completed", "Pending", etc.
  totalEvaluations: number;
  completedEvaluations: number;
  pendingEvaluations: number;
}
