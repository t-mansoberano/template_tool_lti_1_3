# Ventajas de la Arquitectura

## Separación de responsabilidades
Cada capa tiene un propósito claro:
- `Application`: Lógica central y contratos. No depende de frameworks ni detalles de implementación.
- `Infrastructure`: Maneja integraciones externas sin exponer su implementación al resto del sistema.
- `Server`: Gestiona el ciclo de vida HTTP y expone las funcionalidades.
- `Client`: Maneja la interacción del usuario con el sistema.

Esto permite que cambios en una capa no impacten las demás.

---

## Estructura uniforme (inspirada en Screaming Architecture)
Cada capa organiza sus archivos en estructuras predecibles y consistentes, lo que permite a los desarrolladores localizar rápidamente los componentes necesarios para realizar modificaciones o depuración. Esta uniformidad no solo ahorra tiempo, sino que también reduce errores, ya que los equipos pueden anticipar dónde encontrar funcionalidades específicas. Al utilizar estructuras familiares en todas las capas, el proceso de incorporación de nuevos desarrolladores también se ve facilitado, fomentando una colaboración más eficiente.

Esto mejora significativamente la productividad del equipo al permitir localizar rápidamente archivos relacionados con una tarea específica y facilita el mantenimiento al reducir errores comunes causados por la falta de uniformidad. Al emplear una estructura consistente, los desarrolladores pueden anticipar la ubicación de elementos clave, optimizando así el flujo de trabajo.

---

## “Cliente tonto”
La capa Client se encarga exclusivamente de la obtención y presentación de datos, así como del envío de acciones al servidor (Server). En arquitecturas como MVP (Model-View-Presenter), el Client actúa como una vista simplificada o "tonta", enfocándose únicamente en la representación visual y delegando toda la lógica al Presenter o al backend. Este enfoque, combinado con Clean Architecture y Screaming Architecture, refuerza la separación de responsabilidades al abstraer la lógica de negocio en capas internas más robustas, manteniendo al cliente simple y predecible.

Esta centralización de la lógica de negocio en el backend ofrece múltiples ventajas:
- Mejora la cohesión del sistema al agrupar reglas de negocio en un único lugar.
- Reduce duplicidades al evitar que la lógica de negocio se replique en múltiples capas.
- Facilita la escalabilidad del sistema al permitir cambios consistentes en la lógica de negocio sin generar conflictos entre capas.
- Simplifica el mantenimiento del cliente, haciéndolo menos propenso a errores y más sencillo de probar.

Este diseño enfatiza principios clave como modularidad, bajo acoplamiento y alta cohesión, garantizando sistemas escalables, mantenibles y alineados con las necesidades del dominio del negocio.

---

## Extensibilidad
- Las interfaces en `Application` permiten agregar nuevos servicios o reemplazar implementaciones existentes sin modificar el código de otras capas.
- Por ejemplo, se podría cambiar la implementación de `ICanvasApiClient` sin alterar los controladores en `Server`.

---

## Testeabilidad
- Las dependencias están inyectadas mediante DI (Dependency Injection), lo que facilita pruebas unitarias y de integración.
- La lógica de negocio se encuentra aislada, lo que permite crear pruebas de los casos de uso (`GetCompleteEvaluationsViewQuery`) sin necesidad de ejecutar la infraestructura completa.

---

## Desacoplamiento
- Las dependencias entre capas están gestionadas a través de contratos e inyección de dependencias.
- La lógica de negocio no depende directamente de la infraestructura, lo que minimiza el impacto de cambios en servicios externos como Canvas.

---

## Reutilización
- Los servicios como `CanvasApiClient` o `JwtValidationService` pueden ser reutilizados en múltiples partes del sistema sin duplicar código.
- Los modelos compartidos (`Enrollment`, `StudentEvaluation`, etc.) facilitan la coherencia en los datos que manejan distintas funcionalidades.
