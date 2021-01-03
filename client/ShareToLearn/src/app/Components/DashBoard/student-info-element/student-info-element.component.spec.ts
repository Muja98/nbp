import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentInfoElementComponent } from './student-info-element.component';

describe('StudentInfoElementComponent', () => {
  let component: StudentInfoElementComponent;
  let fixture: ComponentFixture<StudentInfoElementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StudentInfoElementComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentInfoElementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
