import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CourseSummaryComponent } from './course-summary.component';

describe('CourseSummaryComponent', () => {
  let component: CourseSummaryComponent;
  let fixture: ComponentFixture<CourseSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CourseSummaryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CourseSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
