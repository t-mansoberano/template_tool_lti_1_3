export interface Evaluation {
  course: any; // Datos del curso
  courseState: any; // Resumen del estado del curso
  students: any[]; // Lista de estudiantes
  evaluationStructures: any[]; // Estructura de las evaluaciones
  selectedStudent: any | null; // Estudiante seleccionado
}
