export interface Vehicle {
  id: number;
  vehicleIdentifier: string;
  make: string;
  model: string;
  year: number;
  vin?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface VehicleStats {
  vehicleIdentifier: string;
  totalEvents: number;
  lastEventTime?: string;
  lastKnownSpeed?: number;
  lastKnownLatitude?: number;
  lastKnownLongitude?: number;
  activeTrips: number;
  totalAlerts: number;
}

export interface TelematicsEvent {
  id: number;
  vehicleIdentifier: string;
  timestamp: string;
  latitude?: number;
  longitude?: number;
  speed?: number;
  heading?: number;
  altitude?: number;
  eventType: string;
  additionalData?: string;
  isProcessed?: boolean;
}

export interface TelematicsEventDto {
  vehicleIdentifier: string;
  timestamp: string;
  latitude?: number;
  longitude?: number;
  speed?: number;
  heading?: number;
  altitude?: number;
  odometer?: number;
  fuelLevel?: number;
  engineLoad?: number;
  engineRPM?: number;
  engineCoolantTemperature?: number;
  eventType: string;
  additionalData?: string;
}