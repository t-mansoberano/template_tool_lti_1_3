# Capa `Server` (Exposición de la API y configuración del servidor)

## Responsabilidad
Maneja la configuración del servidor, define controladores para exponer funcionalidades y actúa como un puente entre las capas Application e Infrastructure.

## Descripción y Funcionalidad
La capa Server actúa como el punto de entrada al sistema desde el lado del backend. Gestiona las solicitudes HTTP, configura los servicios esenciales y define los controladores responsables de manejar las rutas y la lógica asociada. Esta capa utiliza los principios de responsabilidad única y delegación de tareas hacia capas más específicas.

## Componentes clave
### 1. Controladores
- `CanvasController`: Exposición de endpoints para autorización OAuth y validación de tokens.
- `LtiController`: Manejo de lanzamientos LTI y obtención de datos de contexto.
- `GetCompleteEvaluationsViewController`: Recupera evaluaciones completas de los cursos.

### Servicios comunes
- `SessionStorageService`: Gestión del almacenamiento de sesión HTTP.
- `AppSettingsService`: Centraliza la configuración del sistema (CORS, LTI, Canvas, etc.).

### Configuración y middleware
- `Startup`: Define servicios, middleware, CORS, Swagger y configuración de Sentry.

## Beneficios
- **Desacoplamiento**: La capa delega lógica a las capas Application e Infrastructure, reduciendo la complejidad.
- **Escalabilidad**: Soporta nuevas funcionalidades agregando controladores o servicios sin afectar los existentes.
- **Flexibilidad**: Configuraciones como Cors, JWT o LTI pueden adaptarse fácilmente según las necesidades del sistema.
