import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupDocumentComponent } from './group-document.component';

describe('GroupDocumentComponent', () => {
  let component: GroupDocumentComponent;
  let fixture: ComponentFixture<GroupDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupDocumentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
