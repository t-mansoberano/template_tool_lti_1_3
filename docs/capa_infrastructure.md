# Capa `Infrastructure` (Integraciones externas)

## Responsabilidad
Implementa los contratos definidos en la capa Application, gestionando la interacción con APIs externas, bases de datos y otros servicios.

## Descripción y Funcionalida
Se encarga de la interacción con recursos externos, como APIs, bases de datos y servicios de terceros. Implementa los contratos definidos en la capa Application.

## Componentes clave
### 1. Integración con API Canvas LMS 
  - `CanvasApiClient`: Cliente HTTP para operaciones con Canvas.
  - `CanvasOAuthService`: Maneja autenticación y renovación de tokens OAuth.
  - `EnrollmentsService`: Recupera inscripciones en cursos utilizando `ICanvasApiClient`.

### 2. Manejo de JWT (LTI 1.3)
- `JwtValidationService`: Valida tokens JWT provenientes de lanzamientos LTI.

### 3. Federación de usuarios
- `FederationService`: Proporciona un contexto federado para usuarios autenticados.

### 4. Políticas comunes
- `SnakeCaseNamingPolicy`: Convierte nombres de propiedades a formato `snake_case` para interacciones con APIs externas.

### 5. Comunicacion con BD
- Implementación de repositorios para comunicación con la BD

### Beneficios
- **Separación de responsabilidades**: Aísla la lógica de infraestructura para que el resto del sistema no dependa de tecnologías específicas.
- **Reutilización**: Cambiar una API externa o su autenticación solo impacta esta capa.
- **Portabilidad**: Implementa estándares globales como OAuth2 y JWT.
