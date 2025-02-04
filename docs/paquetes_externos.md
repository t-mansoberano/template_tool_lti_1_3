# Paquetes Externos Utilizados

## MediatR
- **Descripción:**
  MediatR es un paquete que implementa el patrón de diseño **Mediador**, utilizado para manejar la comunicación entre componentes sin acoplarlos directamente.
- **Uso en la arquitectura:**
    - Gestiona consultas y comandos en la capa `Application`, como en `GetCompleteEvaluationsViewQuery` y `GetTestForCanvasApiQuery`.
    - Permite separar la lógica de negocio de los controladores y otras partes del sistema.
- **Impacto:**
    - **Desacoplamiento:** Los controladores no contienen lógica de negocio directamente, delegando todo a los handlers de MediatR.
    - **Escalabilidad:** Facilita agregar nuevos casos de uso sin alterar el código existente.
    - **Testeabilidad:** Los handlers se pueden probar de forma aislada, sin necesidad de mockear controladores.

## FluentValidation
- **Descripción:**
  FluentValidation es un paquete que proporciona una forma fluida y expresiva de definir validaciones para modelos y datos de entrada.
- **Uso en la arquitectura:**
    - Validación de datos en la capa `Application`, como en los validadores `GetCompleteEvaluationsViewQueryValidator` y `GetTestForCanvasApiQueryValidator`.
- **Impacto:**
    - **Modularidad:** La validación está desacoplada de la lógica de negocio, lo que mejora la claridad y el mantenimiento.
    - **Reusabilidad:** Las reglas de validación se pueden compartir entre diferentes casos de uso.
    - **Robustez:** Garantiza que los datos procesados cumplen con los requisitos antes de ejecutar lógica compleja.

## Serilog
- **Descripción:**
  Serilog es un sistema avanzado de registro (logging) que permite capturar eventos y enviar los logs a múltiples destinos como archivos, bases de datos o servicios en la nube.
- **Uso en la arquitectura:**
    - Configurado en `LoggingConfiguration` para registrar eventos a nivel del servidor.
    - Captura errores, trazas y métricas de desempeño, además de enviar eventos a Sentry para monitoreo en tiempo real.
- **Impacto:**
    - **Monitoreo:** Facilita identificar problemas en producción mediante logs estructurados.
    - **Depuración:** Los desarrolladores pueden rastrear errores específicos con más detalle.
    - **Escalabilidad:** Soporta múltiples destinos de logs (archivos, Sentry, bases de datos).

## CSharpFunctionalExtensions
- **Descripción:**
  Este paquete simplifica el manejo de valores opcionales, resultados y errores mediante clases como `Result` y `Maybe`.
- **Uso en la arquitectura:**
    - Manejo explícito de resultados en métodos como `HandleRedirectAsync` y `GetStudentsByCourseAsync`.
    - Permite retornar valores exitosos o errores de manera uniforme.
- **Impacto:**
    - **Robustez:** Mejora el manejo de errores y evita excepciones no controladas.
    - **Legibilidad:** Los flujos de datos son más claros y menos propensos a errores.
    - **Consistencia:** Todos los métodos que interactúan con servicios externos devuelven un objeto `Result`.
