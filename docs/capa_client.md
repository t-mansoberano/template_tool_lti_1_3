# Capa `Client` (FrontEnd - Interfaz para el usuario)

## Responsabilidad
Maneja la interacción del usuario con el sistema, implementada con Angular.

## Descripción y Funcionalidad
Implementada con Angular, esta capa representa la interfaz de usuario. Está organizada en módulos y servicios para manejar las interacciones del usuario con el sistema, implementando el patron Contenedor-Presentador o también conocido como Smart and Dumb Component.

## Componentes clave

### 1. Core
- Componentes básicos como manejo de errores `error-page.component.ts` y carga `loading.component.ts`.
- Guardias como `auth.guard.ts` para restringir el acceso a rutas basadas en la autenticación.

### 2. Shared
- Componentes compartidos con otros Features.

### 3. Features
- **Instructors**
  - Divido en `components`, `models`, `services`, `container-component`
- **Students:**
  - Similar a Instructors, pero orientado al perfil de los estudiantes.
- **External Collaborators:**
  - Similar a Instructors & Student, pero orientado al perfil de los administradores externos.

### Beneficios
- **Extensibilidad**: La modularidad permite añadir nuevas funcionalidades fácilmente.
- **Interoperabilidad**: Servicios como `api.service.ts` unifican las llamadas al backend.
