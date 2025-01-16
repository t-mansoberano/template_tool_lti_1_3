import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluationsByCompetenciesComponent } from './evaluations-by-competencies.component';

describe('EvaluationsByCompetenciesComponent', () => {
  let component: EvaluationsByCompetenciesComponent;
  let fixture: ComponentFixture<EvaluationsByCompetenciesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EvaluationsByCompetenciesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EvaluationsByCompetenciesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
