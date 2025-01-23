import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EvaluationComponent } from './evaluation.component';
import { ApiService } from './services/api.service';
import { of, throwError } from 'rxjs';
import { NO_ERRORS_SCHEMA } from '@angular/core';

describe('EvaluationComponent', () => {
  let component: EvaluationComponent;
  let fixture: ComponentFixture<EvaluationComponent>;
  let mockEvaluationDataService: jasmine.SpyObj<ApiService>;

  /**
   * Datos simulados para diferentes escenarios
   */
  const mockEvaluationData: any = {
    students: [{ id: '1', name: 'Student 1' }],
    feedbacks: [{ id: 'f1', text: 'Buen trabajo' }],
    competencies: [{ id: 'c1', name: 'Competencia 1' }]
  };

  const mockCanvasData = { testCanvasData: 'data' };

  beforeEach(async () => {
    /**
     * 1. Crear el mock del servicio con jasmine.createSpyObj
     */
    mockEvaluationDataService = jasmine.createSpyObj<ApiService>(
      'EvaluationDataService',
      ['getEvaluations', 'getTestCanvasAPI']
    );

    /**
     * 2. Configurar valores de retorno por defecto de los métodos del mock
     */
    mockEvaluationDataService.getEvaluations.and.returnValue(of(mockEvaluationData));
    mockEvaluationDataService.getTestCanvasAPI.and.returnValue(of(mockCanvasData));

    await TestBed.configureTestingModule({
      imports: [EvaluationComponent], // Componente standalone
      providers: [
        { provide: ApiService, useValue: mockEvaluationDataService }
      ],
      /**
       * 3. Usar NO_ERRORS_SCHEMA para ignorar errores
       *    de componentes secundarios no declarados
       */
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    /**
     * 4. Crear componente y disparar el ciclo de cambio
     */
    fixture = TestBed.createComponent(EvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  /**
   * Caso base: El componente se crea correctamente
   */
  it('should create', () => {
    expect(component).toBeTruthy();
  });

  /**
   * Interacción con servicios:
   * Verifica que se llamen los métodos del servicio e
   * inyecten correctamente los datos.
   */
  it('should load evaluation data on initialization', () => {
    expect(mockEvaluationDataService.getEvaluations).toHaveBeenCalled();
    expect(component.viewModel).toEqual(mockEvaluationData);
  });

  it('should load canvas data on initialization', () => {
    expect(mockEvaluationDataService.getTestCanvasAPI).toHaveBeenCalled();
    expect(component.canvasData).toEqual(mockCanvasData);
  });

  /**
   * Manejo de estados: datos vacíos
   */
  it('should handle empty evaluation data', () => {
    // Simulamos que el servicio retorna datos vacíos
    mockEvaluationDataService.getEvaluations.and.returnValue(
      of({ students: [], feedbacks: [], competencies: [] })
    );
    // Recreamos el componente para reflejar este nuevo mock
    fixture = TestBed.createComponent(EvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    expect(component.viewModel).toEqual({
      students: [],
      feedbacks: [],
      competencies: []
    });
  });

  /**
   * Manejo de errores en el servicio
   */
  it('should handle error when service returns an error', () => {
    spyOn(console, 'log'); // Para verificar que se llame a console.log
    // Simulamos un error en el servicio
    mockEvaluationDataService.getEvaluations.and.returnValue(
      throwError(() => new Error('Service Error'))
    );

    // Re-creamos el componente para reflejar este nuevo mock
    fixture = TestBed.createComponent(EvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    // Verificamos que se haya llamado a console.log con el error
    expect(console.log).toHaveBeenCalledWith(new Error('Service Error'));
    // Se asume que, ante el error, evaluation se mantiene en null
    expect(component.viewModel).toBeNull();
  });

  /**
   * Comportamiento de la interfaz: cambio de pestañas
   */
  it('should change active tab when onTabChange is called', () => {
    const newTab = 'Evaluar por competencia/subcompetencia';
    component.onTabChange(newTab);
    expect(component.activeTab).toBe(newTab);
  });

  /**
   * Comportamiento de la interfaz: selección de estudiante
   */
  it('should select a student when onStudentSelected is called', () => {
    component.onStudentSelected('1');
    expect(component.selectedStudent).toEqual({ id: '1', name: 'Student 1' });
  });

  /**
   * Comportamiento de la interfaz: renderizado de competencias
   * En este caso, validamos que se llame el método
   * correspondiente con el nivel evaluado.
   */
  it('should log competency level when onCompetencyEvaluated is called', () => {
    spyOn(console, 'log');
    component.onCompetencyEvaluated('Expert');
    expect(console.log).toHaveBeenCalledWith('Competency evaluated as: Expert');
  });

});
