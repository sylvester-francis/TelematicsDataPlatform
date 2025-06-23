import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NavToolbar } from './nav-toolbar';

describe('NavToolbar', () => {
  let component: NavToolbar;
  let fixture: ComponentFixture<NavToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NavToolbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NavToolbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
