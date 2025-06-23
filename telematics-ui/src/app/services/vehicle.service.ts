import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Vehicle, VehicleStats, TelematicsEvent, TelematicsEventDto } from '../models/vehicle.model';

@Injectable({
  providedIn: 'root'
})
export class VehicleService {
  private apiUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  getVehicles(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(`${this.apiUrl}/vehicles`);
  }

  getVehicleStats(vehicleIdentifier: string): Observable<VehicleStats> {
    return this.http.get<VehicleStats>(`${this.apiUrl}/vehicles/${vehicleIdentifier}/stats`);
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
    return this.http.get<TelematicsEvent[]>(`${this.apiUrl}/telematics/vehicles/${vehicleIdentifier}/events${params}`);
  }

  submitEvent(eventData: TelematicsEventDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/telematics/events`, eventData);
  }

  submitBatchEvents(events: TelematicsEventDto[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/telematics/events/batch`, { events });
  }
}