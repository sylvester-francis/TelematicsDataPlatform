import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { VehicleDetails } from './vehicle-details';

describe('VehicleDetails', () => {
  let component: VehicleDetails;
  let fixture: ComponentFixture<VehicleDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [VehicleDetails],
      providers: [
        provideRouter([]),
        provideHttpClient()
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VehicleDetails);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
