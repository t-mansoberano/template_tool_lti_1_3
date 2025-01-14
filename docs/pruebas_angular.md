# Directrices para Pruebas en Angular

## **Propósito de las pruebas unitarias en el frontend**
El objetivo principal de las pruebas unitarias en el frontend es:
- Verificar que la interfaz de usuario (UI) se comporte correctamente frente a diferentes estados de los datos proporcionados por el backend.
- Validar la interacción del usuario con los elementos de la UI (eventos, cambios de estado, etc.).
- Garantizar que los componentes rendericen correctamente el estado proporcionado por el backend.

⚠️ **Nota**: No es necesario probar lógica de negocio compleja en el frontend, ya que esta reside completamente en el backend.

---

## **Cobertura mínima en las pruebas**

### **1. Interacción con servicios**
- Asegurar que el componente realiza las llamadas necesarias a los servicios inyectados.
- Validar que los datos obtenidos de los servicios se procesan y renderizan correctamente.

### **2. Comportamiento de la interfaz**
- Verificar que la UI responde correctamente a las interacciones del usuario (clics, selección de opciones, etc.).
- Asegurar que los datos dinámicos (listas, tablas, etc.) se renderizan adecuadamente.

### **3. Estados del componente**
- Probar el comportamiento del componente frente a:
    - **Datos vacíos**.
    - **Datos cargados**.
    - **Errores en los servicios**.

### **4. Lógica de renderizado básica**
- Validar que los elementos correctos se renderizan en función del estado actual del componente.

---

## **Estrategia de prueba: reglas prácticas**

### **1. Separación de responsabilidades**
- **Componente**:
    - Llamar al servicio necesario para obtener datos.
    - Actualizar el estado y la vista en función de los datos obtenidos.
    - Emitir eventos si es necesario.
- **Servicios**:
    - Asumir que los servicios funcionan correctamente y simular (mockear) su comportamiento en las pruebas.

### **2. Uso de mocks**
- Simula servicios externos con herramientas como `jasmine.createSpyObj`.
- Configura las respuestas de los mocks utilizando `of()` de RxJS para simular observables.

### **3. Evita probar componentes secundarios directamente**
- Usa `NO_ERRORS_SCHEMA` en las pruebas de componentes padres para ignorar errores relacionados con componentes secundarios.
- Los componentes secundarios deben ser probados individualmente.

### **4. Casos de prueba recomendados**
1. **Renderización inicial**:
    - Verificar que el componente se renderiza sin errores con un estado inicial.
2. **Interacciones del usuario**:
    - Validar que los eventos del usuario (clics, cambios de pestaña, etc.) cambian el estado esperado del componente.
3. **Estados dinámicos**:
    - Asegurar que los datos provenientes de los servicios se renderizan correctamente.
    - Probar estados como datos vacíos, datos cargados y errores.
4. **Integración básica con servicios**:
    - Confirmar que el componente llama a los métodos correctos del servicio.

---

## **Estructura estándar para pruebas unitarias**

Usa esta plantilla para estructurar tus pruebas:

```typescript
describe('ComponentName', () => {
  let component: ComponentName;
  let fixture: ComponentFixture<ComponentName>;
  let mockService: jasmine.SpyObj<YourService>;

  beforeEach(async () => {
    mockService = jasmine.createSpyObj('YourService', ['method1', 'method2']);
    mockService.method1.and.returnValue(of(mockData));

    await TestBed.configureTestingModule({
      declarations: [ComponentName], // Declaración del componente
      providers: [
        { provide: YourService, useValue: mockService }, // Inyectar el mock del servicio
      ],
      schemas: [NO_ERRORS_SCHEMA], // Ignorar componentes secundarios
    }).compileComponents();

    fixture = TestBed.createComponent(ComponentName);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call method1 on initialization', () => {
    expect(mockService.method1).toHaveBeenCalled();
  });

  it('should render data correctly', () => {
    component.data = mockData;
    fixture.detectChanges();
    const element = fixture.nativeElement.querySelector('.data-element');
    expect(element.textContent).toContain('Expected Text');
  });

  it('should handle user interaction', () => {
    const button = fixture.nativeElement.querySelector('button');
    button.click();
    expect(component.someState).toBe('expectedState');
  });
});
```

### **Reglas de oro**
1. Evita pruebas redundantes:
    - No pruebes lógica de negocio compleja en el frontend; esto es responsabilidad del backend.
2. Simplicidad en las pruebas:
    - Escribe pruebas fáciles de entender y mantener.
3. Cubre estados, no implementaciones:
    - Probar cómo el componente responde a diferentes entradas y eventos, no cómo está implementado internamente.

### **Mantenimiento de las pruebas**
1. Reutiliza mocks para servicios en todo el proyecto.
2. Usa helpers comunes para evitar duplicación de código en las pruebas.
3. Mantén las pruebas actualizadas conforme el componente evoluciona.

### **Conclusión**
Siguiendo estos lineamientos, las pruebas unitarias en el frontend estarán alineadas con el enfoque de mantener al cliente como una vista tonta. Esto garantiza un desarrollo más ágil, pruebas consistentes y componentes fáciles de mantener.