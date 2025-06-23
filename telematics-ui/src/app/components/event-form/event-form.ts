import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { VehicleService } from '../../services/vehicle.service';
import { TelematicsEventDto } from '../../models/vehicle.model';

@Component({
  selector: 'app-event-form',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatSnackBarModule
  ],
  templateUrl: './event-form.html',
  styleUrl: './event-form.scss'
})
export class EventForm {
  eventForm: FormGroup;
  eventTypes = ['POSITION', 'IGNITION_ON', 'IGNITION_OFF', 'SPEEDING', 'HARSH_BRAKING', 'HARSH_ACCELERATION'];
  
  constructor(
    private fb: FormBuilder,
    private vehicleService: VehicleService,
    private snackBar: MatSnackBar
  ) {
    this.eventForm = this.fb.group({
      vehicleIdentifier: ['', Validators.required],
      timestamp: [new Date().toISOString().slice(0, 16), Validators.required],
      latitude: [''],
      longitude: [''],
      speed: [''],
      heading: [''],
      altitude: [''],
      odometer: [''],
      fuelLevel: [''],
      engineLoad: [''],
      engineRPM: [''],
      engineCoolantTemperature: [''],
      eventType: ['POSITION', Validators.required],
      additionalData: ['']
    });
  }

  onSubmit(): void {
    if (this.eventForm.valid) {
      const eventData: TelematicsEventDto = {
        ...this.eventForm.value,
        timestamp: new Date(this.eventForm.value.timestamp).toISOString()
      };

      this.vehicleService.submitEvent(eventData).subscribe({
        next: (response) => {
          this.snackBar.open('Event submitted successfully!', 'Close', {
            duration: 3000,
            panelClass: ['success-snackbar']
          });
          this.eventForm.reset();
          this.eventForm.patchValue({
            timestamp: new Date().toISOString().slice(0, 16),
            eventType: 'POSITION'
          });
        },
        error: (error) => {
          this.snackBar.open('Error submitting event. Please try again.', 'Close', {
            duration: 3000,
            panelClass: ['error-snackbar']
          });
          console.error('Error submitting event:', error);
        }
      });
    }
  }

  fillSampleData(): void {
    this.eventForm.patchValue({
      vehicleIdentifier: 'DEMO-001',
      latitude: 40.7128,
      longitude: -74.0060,
      speed: 65,
      heading: 45,
      altitude: 100,
      odometer: 125000,
      fuelLevel: 75,
      engineLoad: 45,
      engineRPM: 2500,
      engineCoolantTemperature: 85,
      eventType: 'POSITION'
    });
  }
}
