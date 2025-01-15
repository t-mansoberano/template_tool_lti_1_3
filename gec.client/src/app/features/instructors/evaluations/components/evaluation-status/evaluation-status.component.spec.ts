import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EvaluationStatusComponent } from './evaluation-status.component';

describe('EvaluationStatusComponent', () => {
  let component: EvaluationStatusComponent;
  let fixture: ComponentFixture<EvaluationStatusComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EvaluationStatusComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EvaluationStatusComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
