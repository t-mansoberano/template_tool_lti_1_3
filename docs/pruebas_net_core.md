# Directrices para Pruebas Unitarias en .NET Core con NUnit

## **Formato de Nombres de Métodos**
- Utilizar el formato: `MethodName_ShouldExpectedBehavior_WhenCondition`.
    - **Ejemplo:** `Handle_ShouldReturnSuccess_WhenValidCourseIdProvided`
- Los nombres deben ser descriptivos, claros y reflejar el comportamiento que se está probando.

## **Uso del Patrón AAA (Arrange, Act, Assert)**
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

## **Configuración Común**
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

## **Constantes Compartidas**
- Declarar valores comunes como constantes descriptivas en una clase estática compartida.

**Ejemplo:**
```csharp
public static class TestConstants
{
    public const string VALID_COURSE_ID = "123";
    public const string SERVICE_ERROR = "Service error";
}
```

## **Datos de Prueba Reutilizables**
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

# **Herramientas y Librerías Recomendadas**

## **Uso de FluentAssertions**
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

## **Validación de Mocks**
- Verificar no solo que se llamaron los métodos esperados, sino también con los parámetros correctos.

**Ejemplo:**
```csharp
_mockEnrollmentsService.Verify(s => s.GetStudentsByCourseAsync("123"), Times.Once);
```

## **Organización del Código de Pruebas**

## **Estructura de Carpetas**
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

## **Separación de Casos de Prueba**
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