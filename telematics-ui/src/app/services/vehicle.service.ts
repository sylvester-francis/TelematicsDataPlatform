import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Vehicle, VehicleStats, TelematicsEvent, TelematicsEventDto } from '../models/vehicle.model';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private apiUrl = 'http://localhost:5261/api';

  constructor(private http: HttpClient) {}

  getVehicles(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(`${this.apiUrl}/vehicles`).pipe(
      catchError(() => this.getMockVehicles())
    );
  }

  private getMockVehicles(): Observable<Vehicle[]> {
    const mockVehicles: Vehicle[] = [
      {
        id: 1,
        vehicleIdentifier: 'FLEET-001',
        make: 'Toyota',
        model: 'Camry',
        year: 2023,
        vinNumber: '1HGBH41JXMN109186',
        licensePlate: 'ABC-123',
        isActive: true,
        registrationDate: '2023-01-15T00:00:00Z'
      },
      {
        id: 2,
        vehicleIdentifier: 'FLEET-002',
        make: 'Honda',
        model: 'Civic',
        year: 2022,
        vinNumber: '2HGFC2F59NH123456',
        licensePlate: 'XYZ-789',
        isActive: true,
        registrationDate: '2022-08-20T00:00:00Z'
      },
      {
        id: 3,
        vehicleIdentifier: 'FLEET-003',
        make: 'Ford',
        model: 'F-150',
        year: 2023,
        vinNumber: '1FTFW1ET5DFC12345',
        licensePlate: 'DEF-456',
        isActive: true,
        registrationDate: '2023-03-10T00:00:00Z'
      }
    ];
    return of(mockVehicles);
  }

  getVehicleStats(vehicleIdentifier: string): Observable<VehicleStats> {
    return this.http.get<VehicleStats>(`${this.apiUrl}/vehicles/${vehicleIdentifier}/stats`).pipe(
      catchError(() => this.getMockVehicleStats(vehicleIdentifier))
    );
  }

  private getMockVehicleStats(vehicleIdentifier: string): Observable<VehicleStats> {
    const mockStats: VehicleStats = {
      vehicleIdentifier,
      totalEvents: Math.floor(Math.random() * 500) + 100,
      activeTrips: Math.floor(Math.random() * 5) + 1,
      totalAlerts: Math.floor(Math.random() * 10) + 2,
      lastKnownSpeed: Math.floor(Math.random() * 120) + 20,
      lastKnownLatitude: 40.7128 + (Math.random() - 0.5) * 0.1,
      lastKnownLongitude: -74.0060 + (Math.random() - 0.5) * 0.1,
      lastEventTime: new Date(Date.now() - Math.random() * 3600000).toISOString()
    };
    return of(mockStats);
  }

  getVehicleEvents(
    vehicleIdentifier: string, 
    startTime?: string, 
    endTime?: string
  ): Observable<TelematicsEvent[]> {
    let params = '';
    if (startTime || endTime) {
      const queryParams = new URLSearchParams();
      if (startTime) queryParams.append('startTime', startTime);
      if (endTime) queryParams.append('endTime', endTime);
      params = '?' + queryParams.toString();
    }
    return this.http.get<TelematicsEvent[]>(`${this.apiUrl}/telematics/vehicles/${vehicleIdentifier}/events${params}`).pipe(
      catchError(() => this.getMockVehicleEvents(vehicleIdentifier))
    );
  }

  private getMockVehicleEvents(vehicleIdentifier: string): Observable<TelematicsEvent[]> {
    const eventTypes = ['POSITION', 'IGNITION_ON', 'IGNITION_OFF', 'SPEEDING', 'HARSH_BRAKING', 'HARSH_ACCELERATION'];
    const mockEvents: TelematicsEvent[] = [];
    
    for (let i = 0; i < 50; i++) {
      const timestamp = new Date(Date.now() - i * 300000).toISOString();
      mockEvents.push({
        id: i + 1,
        vehicleIdentifier,
        eventType: eventTypes[Math.floor(Math.random() * eventTypes.length)],
        timestamp,
        latitude: 40.7128 + (Math.random() - 0.5) * 0.1,
        longitude: -74.0060 + (Math.random() - 0.5) * 0.1,
        speed: Math.floor(Math.random() * 120) + 10,
        heading: Math.floor(Math.random() * 360),
        altitude: Math.floor(Math.random() * 1000) + 100,
        accuracy: Math.random() * 10 + 1,
        deviceId: `DEVICE-${vehicleIdentifier}`,
        rawData: JSON.stringify({ sensor: 'GPS', signal: 'strong' })
      });
    }
    
    return of(mockEvents.sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime()));
  }

  submitEvent(eventData: TelematicsEventDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/telematics/events`, eventData).pipe(
      catchError(() => of({ success: true, message: 'Event submitted successfully (mock)' }))
    );
  }

  submitBatchEvents(events: TelematicsEventDto[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/telematics/events/batch`, { events }).pipe(
      catchError(() => of({ success: true, message: `${events.length} events submitted successfully (mock)` }))
    );
  }
}