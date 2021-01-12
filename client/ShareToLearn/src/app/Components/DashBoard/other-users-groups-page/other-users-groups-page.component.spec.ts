import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherUsersGroupsPageComponent } from './other-users-groups-page.component';

describe('OtherUsersGroupsPageComponent', () => {
  let component: OtherUsersGroupsPageComponent;
  let fixture: ComponentFixture<OtherUsersGroupsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ OtherUsersGroupsPageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherUsersGroupsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
