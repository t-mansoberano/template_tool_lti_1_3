# Plantilla de solución en .NET Core + Angular App, como herremienta LTI 1.3 integrada en Canvas con peticiones a Canvas API 

# Informe de la Arquitectura del Sistema

## **1. Introducción**

La arquitectura de este sistema está basada en los principios de **Clean Architecture**, que promueve la separación de responsabilidades y un diseño modular. Este enfoque garantiza que los componentes sean fáciles de mantener, probar y escalar. Las capas `Application`, `Infrastructure`, `Server`, `Client` están claramente delimitadas, mejorando la cohesión interna y reduciendo el acoplamiento entre módulos.

### **1.1. Antecedentes y Justificación**

Una buena arquitectura debe cumplir con principios clave como:
- **Bajo acoplamiento y alta cohesión**: Las partes del sistema interactúan de manera controlada, minimizando dependencias.
- **Modularidad**: Facilita agregar, eliminar o modificar componentes.
- **Testeabilidad**: Los módulos aislados son más fáciles de probar.
- **Escalabilidad**: Soporta el crecimiento de funcionalidad y usuarios sin comprometer el rendimiento.
- **Legibilidad y consistencia**: Estructuras predecibles facilitan la colaboración y el mantenimiento.

Referentes como Grady Booch han defendido el uso de arquitecturas orientadas a objetos y principios de diseño para mantener sistemas adaptables y sostenibles. La arquitectura propuesta adopta estos principios y los refuerza con una organización consistente de cada capa.

---

## **2. Estructura de la arquitectura**

### **2.1. Capas**

1. **Capa `Application` (Lógica de negocio y casos de uso)**
    - **Responsabilidad:** Contiene la lógica central del sistema y los contratos necesarios para que otras capas interactúen sin conocer detalles de implementación.
    - **Descripción y Funcionalidad:** Esta capa representa el núcleo de la lógica del negocio y actúa como puente entre Server e Infrastructure. Aquí se definen los contratos (interfaces) y se implementan los patrones de diseño necesarios para garantizar una interacción limpia con otras capas.
    - **Componentes clave:**
        - **Contratos (Contracts):**
          Define las interfaces (`ICanvasApiClient`, `ICanvasOAuthService`, `IFederationService`, etc.) que otras capas deben implementar.
        - **Casos de uso (Features):**
          Implementa consultas y operaciones específicas como:
            - `GetCompleteEvaluationsViewQuery`: Recupera la vista completa de evaluaciones por curso.
            - `GetTestForCanvasApiQuery`: Ejecuta pruebas contra APIs de Canvas.
            - Las funcionalidades están divididas en módulos como:
              - Instructors: Contiene lógicas para manejar evaluaciones (ejemplo: GetCompleteEvaluationsView).
              - Students: Contiene funcionalidades específicas para los estudiantes.
        - **Extensiones y validaciones:**
            - `ValidationResultExtension`: Facilita la interpretación de errores de validación.
   - **Beneficios:**
     - **Modularidad**: Los contratos y módulos están organizados por responsabilidad, lo que simplifica el mantenimiento.
     - **Reutilización**: Los contratos son fácilmente reutilizables por otras capas o sistemas.
     - **Portabilidad**: Cambiar la implementación de un contrato no afecta al resto del sistema.

2. **Capa `Infrastructure` (Integraciones externas)**
    - **Responsabilidad:** Implementa los contratos definidos en la capa Application, gestionando la interacción con APIs externas, bases de datos y otros servicios.
    - **Descripción y Funcionalidad:** Se encarga de la interacción con recursos externos, como APIs, bases de datos y servicios de terceros. Implementa los contratos definidos en la capa Application.
    - **Componentes clave:**
        - **Integración con Canvas LMS:**
            - `CanvasApiClient`: Cliente HTTP para operaciones con Canvas.
            - `CanvasOAuthService`: Maneja autenticación y renovación de tokens OAuth.
            - `EnrollmentsService`: Recupera inscripciones en cursos utilizando `ICanvasApiClient`.
        - **Manejo de JWT (LTI 1.3):**
            - `JwtValidationService`: Valida tokens JWT provenientes de lanzamientos LTI.
        - **Federación de usuarios:**
            - `FederationService`: Proporciona un contexto federado para usuarios autenticados.
        - **Políticas comunes:**
            - `SnakeCaseNamingPolicy`: Convierte nombres de propiedades a formato `snake_case` para interacciones con APIs externas.
   - **Beneficios:**
       - **Separación de responsabilidades**: Aísla la lógica de infraestructura para que el resto del sistema no dependa de tecnologías específicas.
       - **Reutilización**: Cambiar una API externa o su autenticación solo impacta esta capa.
       - **Portabilidad**: Implementa estándares globales como OAuth2 y JWT.

3. **Capa `Server` (Exposición de la API y configuración del servidor)**
    - **Responsabilidad:** Maneja la configuración del servidor, define controladores para exponer funcionalidades y actúa como un puente entre las capas Application e Infrastructure.
    - **Descripción y Funcionalidad:** La capa Server actúa como el punto de entrada al sistema desde el lado del backend. Gestiona las solicitudes HTTP, configura los servicios esenciales y define los controladores responsables de manejar las rutas y la lógica asociada. Esta capa utiliza los principios de responsabilidad única y delegación de tareas hacia capas más específicas.
    - **Componentes clave:**
        - **Controladores:**
            - `CanvasController`: Exposición de endpoints para autorización OAuth y validación de tokens.
            - `LtiController`: Manejo de lanzamientos LTI y obtención de datos de contexto.
            - `GetCompleteEvaluationsViewController`: Recupera evaluaciones completas de los cursos.
        - **Servicios comunes:**
            - `SessionStorageService`: Gestión del almacenamiento de sesión HTTP.
            - `AppSettingsService`: Centraliza la configuración del sistema (CORS, LTI, Canvas, etc.).
        - **Configuración y middleware:**
            - `Startup`: Define servicios, middleware, CORS, Swagger y configuración de Sentry.
   - **Beneficios:**
       - **Desacoplamiento**: La capa delega lógica a las capas Application e Infrastructure, reduciendo la complejidad.
       - **Escalabilidad**: Soporta nuevas funcionalidades agregando controladores o servicios sin afectar los existentes.
       - **Flexibilidad**: Configuraciones como Cors, JWT o LTI pueden adaptarse fácilmente según las necesidades del sistema.


4. **Capa `Client` (FrontEnd - Interfaz para el usuario)**
    - **Responsabilidad:** Maneja la interacción del usuario con el sistema.
    - **Descripción y Funcionalidad:** Implementada con Angular, esta capa representa la interfaz de usuario. Está organizada en módulos y servicios para manejar las interacciones del usuario con el sistema, implementando el patron Contenedor-Presentador o también conocido como Smart and Dumb Component.
    - **Componentes clave:**
        - **Core:**
            - Componentes básicos como manejo de errores `error-page.component.ts` y carga `loading.component.ts`.
            - Guardias como `auth.guard.ts` para restringir el acceso a rutas basadas en la autenticación.
        - **Features:**
            - **Instructors**
              - Divido en `components`, `models`, `services`, `container-component`
            - **Students:**
                - Similar a Instructors, pero orientado al perfil de los estudiantes.
    - **Beneficios:**
        - **Extensibilidad**: La modularidad permite añadir nuevas funcionalidades fácilmente.
        - **Interoperabilidad**: Servicios como `api.service.ts` unifican las llamadas al backend.

---

## **3. Ventajas de esta arquitectura**

### **3.1. Separación de responsabilidades**
Cada capa tiene un propósito claro:
- `Application`: Lógica central y contratos. No depende de frameworks ni detalles de implementación.
- `Infrastructure`: Maneja integraciones externas sin exponer su implementación al resto del sistema.
- `Server`: Gestiona el ciclo de vida HTTP y expone las funcionalidades.
- `Client`: Maneja la interacción del usuario con el sistema.

Esto permite que cambios en una capa no impacten las demás.

---

### **3.2. Estructura uniforme (inspirada en Screaming Architecture)**
Cada capa organiza sus archivos en estructuras predecibles y consistentes, lo que permite a los desarrolladores localizar rápidamente los componentes necesarios para realizar modificaciones o depuración. Esta uniformidad no solo ahorra tiempo, sino que también reduce errores, ya que los equipos pueden anticipar dónde encontrar funcionalidades específicas. Al utilizar estructuras familiares en todas las capas, el proceso de incorporación de nuevos desarrolladores también se ve facilitado, fomentando una colaboración más eficiente.

Esto mejora significativamente la productividad del equipo al permitir localizar rápidamente archivos relacionados con una tarea específica y facilita el mantenimiento al reducir errores comunes causados por la falta de uniformidad. Al emplear una estructura consistente, los desarrolladores pueden anticipar la ubicación de elementos clave, optimizando así el flujo de trabajo.

---

### **3.3. “Cliente tonto”**
La capa Client se encarga exclusivamente de la obtención y presentación de datos, así como del envío de acciones al servidor (Server). En arquitecturas como MVP (Model-View-Presenter), el Client actúa como una vista simplificada o "tonta", enfocándose únicamente en la representación visual y delegando toda la lógica al Presenter o al backend. Este enfoque, combinado con Clean Architecture y Screaming Architecture, refuerza la separación de responsabilidades al abstraer la lógica de negocio en capas internas más robustas, manteniendo al cliente simple y predecible.

Esta centralización de la lógica de negocio en el backend ofrece múltiples ventajas:
- Mejora la cohesión del sistema al agrupar reglas de negocio en un único lugar.
- Reduce duplicidades al evitar que la lógica de negocio se replique en múltiples capas.
- Facilita la escalabilidad del sistema al permitir cambios consistentes en la lógica de negocio sin generar conflictos entre capas.
- Simplifica el mantenimiento del cliente, haciéndolo menos propenso a errores y más sencillo de probar.

Este diseño enfatiza principios clave como modularidad, bajo acoplamiento y alta cohesión, garantizando sistemas escalables, mantenibles y alineados con las necesidades del dominio del negocio.

---

### **3.4. Extensibilidad**
- Las interfaces en `Application` permiten agregar nuevos servicios o reemplazar implementaciones existentes sin modificar el código de otras capas.
- Por ejemplo, se podría cambiar la implementación de `ICanvasApiClient` sin alterar los controladores en `Server`.

---

### **3.5. Testeabilidad**
- Las dependencias están inyectadas mediante DI (Dependency Injection), lo que facilita pruebas unitarias y de integración.
- La lógica de negocio se encuentra aislada, lo que permite crear pruebas de los casos de uso (`GetCompleteEvaluationsViewQuery`) sin necesidad de ejecutar la infraestructura completa.

---

### **3.6. Desacoplamiento**
- Las dependencias entre capas están gestionadas a través de contratos e inyección de dependencias.
- La lógica de negocio no depende directamente de la infraestructura, lo que minimiza el impacto de cambios en servicios externos como Canvas.

---

### **3.7. Reutilización**
- Los servicios como `CanvasApiClient` o `JwtValidationService` pueden ser reutilizados en múltiples partes del sistema sin duplicar código.
- Los modelos compartidos (`Enrollment`, `StudentEvaluation`, etc.) facilitan la coherencia en los datos que manejan distintas funcionalidades.

---

## **4. Justificación técnica**

### **4.1. Clean Architecture**
Esta propuesta cumple los principios de Clean Architecture:
- **Independencia de frameworks:** La lógica no depende de detalles específicos del framework (e.g., ASP.NET Core, Angular).
- **Centralidad de la lógica de negocio:** La lógica de la aplicación está en `Application`, independiente de la infraestructura.
- **Dependencias dirigidas hacia adentro:** Las capas externas (`Server`, `Infrastructure`) dependen de las internas (`Application`).

---

### **4.2. Uso de patrones de diseño**
- **CQRS (Command Query Responsibility Segregation):**
    - Implementado en `Application.Features` para manejar consultas (`GetCompleteEvaluationsViewQuery`).
- **Repository Pattern:**
    - Aunque no explícito, las interfaces como `ICanvasApiClient` y `IEnrollmentsService` se comportan como repositorios abstractos.
- **Factory Method:**
    - Usado para configurar clientes HTTP en `CanvasApiClient`.

---

### **4.3. Escalabilidad**
- La modularidad del sistema permite agregar nuevos módulos (e.g., soporte para otro LMS o servicio) sin reestructurar el proyecto.
- Las configuraciones centralizadas (`appsettings.json`) y la inyección de dependencias facilitan la adaptación a entornos distintos.

---

## **5. Paquetes externos utilizados y su impacto en la arquitectura**

### **5.1. MediatR**
- **Descripción:**
  MediatR es un paquete que implementa el patrón de diseño **Mediador**, utilizado para manejar la comunicación entre componentes sin acoplarlos directamente.
- **Uso en la arquitectura:**
    - Gestiona consultas y comandos en la capa `Application`, como en `GetCompleteEvaluationsViewQuery` y `GetTestForCanvasApiQuery`.
    - Permite separar la lógica de negocio de los controladores y otras partes del sistema.
- **Impacto:**
    - **Desacoplamiento:** Los controladores no contienen lógica de negocio directamente, delegando todo a los handlers de MediatR.
    - **Escalabilidad:** Facilita agregar nuevos casos de uso sin alterar el código existente.
    - **Testeabilidad:** Los handlers se pueden probar de forma aislada, sin necesidad de mockear controladores.

### **5.2. FluentValidation**
- **Descripción:**
  FluentValidation es un paquete que proporciona una forma fluida y expresiva de definir validaciones para modelos y datos de entrada.
- **Uso en la arquitectura:**
    - Validación de datos en la capa `Application`, como en los validadores `GetCompleteEvaluationsViewQueryValidator` y `GetTestForCanvasApiQueryValidator`.
- **Impacto:**
    - **Modularidad:** La validación está desacoplada de la lógica de negocio, lo que mejora la claridad y el mantenimiento.
    - **Reusabilidad:** Las reglas de validación se pueden compartir entre diferentes casos de uso.
    - **Robustez:** Garantiza que los datos procesados cumplen con los requisitos antes de ejecutar lógica compleja.

### **5.3. Serilog**
- **Descripción:**
  Serilog es un sistema avanzado de registro (logging) que permite capturar eventos y enviar los logs a múltiples destinos como archivos, bases de datos o servicios en la nube.
- **Uso en la arquitectura:**
    - Configurado en `LoggingConfiguration` para registrar eventos a nivel del servidor.
    - Captura errores, trazas y métricas de desempeño, además de enviar eventos a Sentry para monitoreo en tiempo real.
- **Impacto:**
    - **Monitoreo:** Facilita identificar problemas en producción mediante logs estructurados.
    - **Depuración:** Los desarrolladores pueden rastrear errores específicos con más detalle.
    - **Escalabilidad:** Soporta múltiples destinos de logs (archivos, Sentry, bases de datos).

### **5.4. CSharpFunctionalExtensions**
- **Descripción:**
  Este paquete simplifica el manejo de valores opcionales, resultados y errores mediante clases como `Result` y `Maybe`.
- **Uso en la arquitectura:**
    - Manejo explícito de resultados en métodos como `HandleRedirectAsync` y `GetStudentsByCourseAsync`.
    - Permite retornar valores exitosos o errores de manera uniforme.
- **Impacto:**
    - **Robustez:** Mejora el manejo de errores y evita excepciones no controladas.
    - **Legibilidad:** Los flujos de datos son más claros y menos propensos a errores.
    - **Consistencia:** Todos los métodos que interactúan con servicios externos devuelven un objeto `Result`.

---

## **6. Conclusión**
La arquitectura propuesta es robusta, escalable y sostenible. Su diseño modular basado en Clean Architecture garantiza que:
- El sistema sea fácil de mantener y probar.
- Los cambios en la infraestructura o servicios externos no afecten la lógica de negocio.
- El desarrollo futuro sea eficiente gracias a la reutilización de componentes.

Este diseño es ideal para sistemas que requieren integraciones externas complejas (como Canvas LMS), alta cohesión y bajo acoplamiento.

## **7. Anexos**
### 7.1 **Diagrama de Dominio**
![DomainModel-Modelo_de_Dominio__Gestor_de_Evaluaciones_por_Competencias.png](4%2B1View/DomainModel-Modelo_de_Dominio__Gestor_de_Evaluaciones_por_Competencias.png)

### 7.2 **Diagrama de Arquitectura**
![Architecture-Diagrama_de_Arquitectura.png](4%2B1View/Architecture-Diagrama_de_Arquitectura.png)

### 7.3 Directrices para Pruebas Unitarias en .NET Core 8 con NUnit

Proporciona un conjunto de estándares y buenas prácticas para garantizar la calidad, consistencia y mantenibilidad de las pruebas unitarias en los proyectos desarrollados por el equipo.

### **Formato de Nombres de Métodos**
- Utilizar el formato: `MethodName_ShouldExpectedBehavior_WhenCondition`.
    - **Ejemplo:** `Handle_ShouldReturnSuccess_WhenValidCourseIdProvided`
- Los nombres deben ser descriptivos, claros y reflejar el comportamiento que se está probando.

### **Uso del Patrón AAA (Arrange, Act, Assert)**
Organizar el cuerpo de las pruebas en tres secciones bien definidas:
1. **Arrange:** Configurar el entorno, mocks y datos necesarios para la prueba.
2. **Act:** Invocar el método bajo prueba.
3. **Assert:** Verificar los resultados esperados.

**Ejemplo:**
```csharp
[Test]
public async Task Handle_ShouldReturnSuccess_WhenValidCourseIdProvided()
{
    // Arrange
    const string validCourseId = "123";
    var query = new GetTestForCanvasApiQuery { CourseId = validCourseId };
    var expectedEnrollments = GetSampleEnrollments(validCourseId);

    _mockEnrollmentsService.Setup(s => s.GetStudentsByCourseAsync(validCourseId))
                           .ReturnsAsync(Result.Success(expectedEnrollments));

    // Act
    var result = await _handler.Handle(query, CancellationToken.None);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Enrollments.Should().BeEquivalentTo(expectedEnrollments);
}
```

## **Reutilización de Código**

### **Configuración Común**
- Crear métodos auxiliares para configurar mocks y datos de prueba comunes.
- Usar la anotación `[SetUp]` para inicializar configuraciones que se comparten entre las pruebas de una misma clase.

**Ejemplo:**
```csharp
[SetUp]
public void SetUp()
{
    _mockEnrollmentsService = new Mock<IEnrollmentsService>();
    _handler = new GetTestForCanvasApiHandle(_mockEnrollmentsService.Object);
}
```

### **Constantes Compartidas**
- Declarar valores comunes como constantes descriptivas en una clase estática compartida.

**Ejemplo:**
```csharp
public static class TestConstants
{
    public const string VALID_COURSE_ID = "123";
    public const string SERVICE_ERROR = "Service error";
}
```

### **Datos de Prueba Reutilizables**
- Crear métodos o clases que proporcionen datos de prueba predefinidos.

**Ejemplo:**
```csharp
private static List<Enrollment> GetSampleEnrollments(string courseId)
{
    return new List<Enrollment>
    {
        new Enrollment
        {
            Id = 1,
            UserId = 101,
            CourseId = int.Parse(courseId),
            Type = "Student",
            Grades = new Grades
            {
                CurrentGrade = 90,
                FinalGrade = 95,
                HtmlUrl = "http://example.com"
            },
            User = new User { Id = 101, Name = "John Doe" }
        }
    };
}
```

## **Herramientas y Librerías Recomendadas**

### **Uso de FluentAssertions**
- Mejorar la legibilidad de las aserciones usando la biblioteca FluentAssertions.
- Ejemplo de conversión:
    - Antes:
      ```csharp
      Assert.IsTrue(result.IsSuccess);
      Assert.AreEqual(expected, result.Value);
      ```
    - Después:
      ```csharp
      result.IsSuccess.Should().BeTrue();
      result.Value.Should().BeEquivalentTo(expected);
      ```

### **Validación de Mocks**
- Verificar no solo que se llamaron los métodos esperados, sino también con los parámetros correctos.

**Ejemplo:**
```csharp
_mockEnrollmentsService.Verify(s => s.GetStudentsByCourseAsync("123"), Times.Once);
```

## **Organización del Código de Pruebas**

### **Estructura de Carpetas**
- Reflejar la estructura del proyecto principal.
- Ejemplo:
  ```plaintext
  - Application
      - Features
          - Instructors
              - Evaluations
                  - Queries
                      - GetCompleteEvaluationsView
                          - GetTestForCanvasApiHandleTests.cs
  - Server
      - Integrations
          - CanvasControllerTests.cs
  ```

### **Separación de Casos de Prueba**
- Cada caso de prueba debe validar un único comportamiento.
- Si un método combina múltiples aserciones, dividirlo en métodos individuales.

## **Comentarios en las Pruebas**
- Agregar comentarios para aclarar casos específicos o lógicas complejas.

**Ejemplo:**
```csharp
// Este caso prueba el comportamiento cuando el servicio devuelve un error.
```

## **Checklist de Revisión**
Antes de integrar las pruebas, asegúrate de que:
- [ ] Los nombres de los métodos siguen el formato estándar.
- [ ] Cada prueba utiliza el patrón AAA.
- [ ] No hay valores mágicos en el código (usar constantes descriptivas).
- [ ] Los mocks están configurados y validados correctamente.
- [ ] Las pruebas son independientes entre sí.
- [ ] Se han cubierto tanto los casos positivos como negativos.

---
