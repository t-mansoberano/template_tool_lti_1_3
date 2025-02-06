export interface StudentModel {
  id: number;
  loginId: string;
  name: string;
  status: string; // Ejemplo: "Completed", "Pending", etc.
  totalEvaluations: number;
  completedEvaluations: number;
  pendingEvaluations: number;
}
