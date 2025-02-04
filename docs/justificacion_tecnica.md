# Justificación Técnica

## Clean Architecture
Esta propuesta cumple los principios de Clean Architecture:
- **Independencia de frameworks:** La lógica no depende de detalles específicos del framework (e.g., ASP.NET Core, Angular).
- **Centralidad de la lógica de negocio:** La lógica de la aplicación está en `Application`, independiente de la infraestructura.
- **Dependencias dirigidas hacia adentro:** Las capas externas (`Server`, `Infrastructure`) dependen de las internas (`Application`).

---

## Uso de patrones de diseño
- **CQRS (Command Query Responsibility Segregation):**
    - Implementado en `Application.Features` para manejar consultas (`GetCompleteEvaluationsViewQuery`) y comandos.
- **Repository Pattern:**
    - Aunque no explícito, las interfaces como `ICanvasApiClient` y `IEnrollmentsService` se comportan como repositorios abstractos.
- **Factory Method:**
    - Usado para configurar clientes HTTP en `CanvasApiClient`.

---

## Escalabilidad
- La modularidad del sistema permite agregar nuevos módulos (e.g., soporte para otro LMS o servicio) sin reestructurar el proyecto.
- Las configuraciones centralizadas (`appsettings.json`) y la inyección de dependencias facilitan la adaptación a entornos distintos.
