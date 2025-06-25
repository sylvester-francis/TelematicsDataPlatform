import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { VehicleService } from '../../services/vehicle.service';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './event-form.html',
  styleUrl: './event-form.scss'
})
export class EventForm implements OnInit {
  eventForm: FormGroup;
  isSubmitting = false;
  submitSuccess = false;
  submitError: string | null = null;
  lastEventId: number | null = null;

  constructor(
    private fb: FormBuilder,
    private vehicleService: VehicleService,
    private router: Router
  ) {
    this.eventForm = this.createForm();
  }

  ngOnInit(): void {
    // Set current timestamp as default
    const now = new Date();
    const localDateTime = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
      .toISOString()
      .slice(0, 16);
    this.eventForm.patchValue({ timestamp: localDateTime });
  }

  private createForm(): FormGroup {
    return this.fb.group({
      // Required fields based on backend validation
      vehicleIdentifier: ['', [Validators.required]],
      timestamp: ['', [Validators.required]],
      
      // Optional location fields
      latitude: [null],
      longitude: [null],
      speed: [null],
      heading: [null],
      altitude: [null],
      
      // Optional vehicle diagnostic fields
      odometer: [null],
      fuelLevel: [null],
      engineLoad: [null],
      engineRPM: [null],
      engineCoolantTemperature: [null],
      
      // Optional event details
      eventType: ['POSITION'], // Default as per backend
      additionalData: ['']
    });
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.eventForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  async onSubmit(): Promise<void> {
    if (this.eventForm.invalid) {
      this.markAllFieldsAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.submitSuccess = false;
    this.submitError = null;

    try {
      // Prepare the payload exactly as expected by backend
      const formValue = this.eventForm.value;
      const payload = {
        vehicleIdentifier: formValue.vehicleIdentifier,
        timestamp: formValue.timestamp,
        ...(formValue.latitude !== null && { latitude: formValue.latitude }),
        ...(formValue.longitude !== null && { longitude: formValue.longitude }),
        ...(formValue.speed !== null && { speed: formValue.speed }),
        ...(formValue.heading !== null && { heading: formValue.heading }),
        ...(formValue.altitude !== null && { altitude: formValue.altitude }),
        ...(formValue.odometer !== null && { odometer: formValue.odometer }),
        ...(formValue.fuelLevel !== null && { fuelLevel: formValue.fuelLevel }),
        ...(formValue.engineLoad !== null && { engineLoad: formValue.engineLoad }),
        ...(formValue.engineRPM !== null && { engineRPM: formValue.engineRPM }),
        ...(formValue.engineCoolantTemperature !== null && { engineCoolantTemperature: formValue.engineCoolantTemperature }),
        eventType: formValue.eventType || 'POSITION',
        ...(formValue.additionalData && { additionalData: formValue.additionalData })
      };

      // Submit to backend
      const response = await firstValueFrom(this.vehicleService.submitEvent(payload));
      
      // Handle success response - backend returns { eventId: number, status: string }
      if (response?.eventId) {
        this.submitSuccess = true;
        this.lastEventId = response.eventId;
        this.clearForm(); // Use clearForm instead of resetForm to avoid clearing success state
        
        // Auto-hide success message after 5 seconds
        setTimeout(() => {
          this.submitSuccess = false;
        }, 5000);
      } else {
        throw new Error('Invalid response from server');
      }

    } catch (error: any) {
      console.error('Event submission failed:', error);
      
      // Handle different error types
      if (error.status === 400) {
        this.submitError = 'Invalid data provided. Please check your inputs.';
      } else if (error.status === 500) {
        this.submitError = 'Server error occurred. Please try again later.';
      } else if (error.error?.title) {
        this.submitError = error.error.title;
      } else {
        this.submitError = 'Failed to submit event. Please try again.';
      }
    } finally {
      this.isSubmitting = false;
    }
  }

  fillSampleData(): void {
    const now = new Date();
    const localDateTime = new Date(now.getTime() - now.getTimezoneOffset() * 60000)
      .toISOString()
      .slice(0, 16);

    this.eventForm.patchValue({
      vehicleIdentifier: 'FLEET-001',
      timestamp: localDateTime,
      latitude: 40.7128,
      longitude: -74.0060,
      speed: 65.5,
      heading: 90,
      altitude: 100,
      odometer: 125000,
      fuelLevel: 75,
      engineLoad: 45,
      engineRPM: 2500,
      engineCoolantTemperature: 85,
      eventType: 'POSITION',
      additionalData: '{"source": "sample", "quality": "high"}'
    });
  }

  clearForm(): void {
    this.eventForm.reset();
    this.eventForm.patchValue({ 
      eventType: 'POSITION',
      timestamp: new Date(Date.now() - new Date().getTimezoneOffset() * 60000)
        .toISOString()
        .slice(0, 16)
    });
    this.submitError = null;
    // Don't reset submitSuccess here - let it show until timeout
  }

  resetForm(): void {
    this.clearForm();
    this.submitSuccess = false;
  }

  private markAllFieldsAsTouched(): void {
    Object.keys(this.eventForm.controls).forEach(key => {
      this.eventForm.get(key)?.markAsTouched();
    });
  }
}