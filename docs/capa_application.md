# Capa `Application` (Lógica de negocio y casos de uso)

## Responsabilidad
Contiene la lógica central del sistema y los contratos necesarios para que otras capas interactúen sin conocer detalles de implementación.

## Descripción y Funcionalida
Esta capa representa el núcleo de la lógica del negocio y actúa como puente entre Server e Infrastructure. Aquí se definen los contratos (interfaces) y se implementan los patrones de diseño necesarios para garantizar una interacción limpia con otras capas.

## Componentes clave
### 1. Contratos (Contracts)
- Define las interfaces (`ICanvasApiClient`, `ICanvasOAuthService`, `IFederationService`, etc.) que otras capas deben implementar.

### 2. Dominios (Features)
- Instructors
- Students
- External Collaborators

### 3. Sub Dominios
- Evaluations

### 2. Casos de uso
- Implementa consultas y operaciones específicas como:
  - `GetCompleteEvaluationsViewQuery`: Recupera la vista completa de evaluaciones por curso.
  - `GetTestForCanvasApiQuery`: Ejecuta pruebas contra APIs de Canvas.
  - Las funcionalidades están divididas en módulos (dominios) como:
    - Instructors: Contiene lógicas para manejar evaluaciones (ejemplo: GetCompleteEvaluationsView).
    - Students: Contiene funcionalidades específicas para los estudiantes.
- Implementa comandos y operaciones específicas

### 3. Extensiones y validaciones
  - `ValidationResultExtension`: Facilita la interpretación de errores de validación.

## Beneficios
- Modularidad: Los contratos y módulos están organizados por responsabilidad, lo que simplifica el mantenimiento.
- Reutilización: Los contratos son fácilmente reutilizables por otras capas o sistemas.
- Portabilidad: Cambiar la implementación de un contrato no afecta al resto del sistema.
