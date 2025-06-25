import { Component, OnInit, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { firstValueFrom } from 'rxjs';
import * as L from 'leaflet';
import { VehicleService } from '../../services/vehicle.service';
import { VehicleStats, TelematicsEvent } from '../../models/vehicle.model';

@Component({
  selector: 'app-vehicle-details',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './vehicle-details.html',
  styleUrl: './vehicle-details.scss'
})
export class VehicleDetails implements OnInit, AfterViewInit {
  @ViewChild('mapContainer', { static: false }) mapContainer!: ElementRef;
  
  vehicleId: string = '';
  vehicleStats: VehicleStats | null = null;
  events: TelematicsEvent[] = [];
  loading = true;
  eventsLoading = false;
  
  private map: L.Map | null = null;
  private vehicleMarker: L.Marker | null = null;


  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private vehicleService: VehicleService
  ) {}

  ngOnInit(): void {
    this.vehicleId = this.route.snapshot.params['id'];
    this.loadVehicleData();
  }

  ngAfterViewInit(): void {
    // Initialize map after view is ready
    if (this.vehicleStats?.lastKnownLatitude && this.vehicleStats?.lastKnownLongitude) {
      this.initializeMap();
    }
  }

  async loadVehicleData(): Promise<void> {
    try {
      this.loading = true;
      
      // Load vehicle statistics
      const stats = await firstValueFrom(this.vehicleService.getVehicleStats(this.vehicleId));
      this.vehicleStats = stats || null;
      
      // Load vehicle events
      await this.loadVehicleEvents();
      
      // Initialize map after data is loaded
      setTimeout(() => {
        if (this.vehicleStats?.lastKnownLatitude && this.vehicleStats?.lastKnownLongitude) {
          this.initializeMap();
        }
      }, 100);
      
    } catch (error) {
      console.error('Error loading vehicle data:', error);
      this.vehicleStats = null;
    } finally {
      this.loading = false;
    }
  }

  async loadVehicleEvents(): Promise<void> {
    try {
      this.eventsLoading = true;
      const events = await firstValueFrom(this.vehicleService.getVehicleEvents(this.vehicleId));
      this.events = events || [];
    } catch (error) {
      console.error('Error loading vehicle events:', error);
      this.events = [];
    } finally {
      this.eventsLoading = false;
    }
  }

  formatDateTime(dateTime: string | undefined): string {
    if (!dateTime) return '-';
    
    try {
      const date = new Date(dateTime);
      return date.toLocaleString('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
      });
    } catch {
      return '-';
    }
  }

  getEventTypeClass(eventType: string): string {
    switch (eventType?.toLowerCase()) {
      case 'position':
        return 'position';
      case 'ignition_on':
        return 'ignition-on';
      case 'ignition_off':
        return 'ignition-off';
      case 'speeding':
        return 'speeding';
      case 'harsh_braking':
        return 'harsh-braking';
      case 'harsh_acceleration':
        return 'harsh-acceleration';
      default:
        return 'default';
    }
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  private initializeMap(): void {
    if (!this.vehicleStats?.lastKnownLatitude || !this.vehicleStats?.lastKnownLongitude) {
      return;
    }

    const lat = this.vehicleStats.lastKnownLatitude;
    const lng = this.vehicleStats.lastKnownLongitude;

    // Create map if it doesn't exist
    if (!this.map && this.mapContainer) {
      this.map = L.map(this.mapContainer.nativeElement, {
        center: [lat, lng],
        zoom: 15,
        zoomControl: true,
        scrollWheelZoom: true
      });

      // Add OpenStreetMap tiles
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© OpenStreetMap contributors',
        maxZoom: 19
      }).addTo(this.map);

      // Fix for Leaflet default marker icons
      this.fixLeafletIcons();
    }

    // Add or update vehicle marker
    if (this.map) {
      if (this.vehicleMarker) {
        this.vehicleMarker.remove();
      }

      // Create custom vehicle icon
      const vehicleIcon = L.divIcon({
        html: `
          <div class="vehicle-marker">
            <div class="marker-icon">🚗</div>
            <div class="marker-pulse"></div>
          </div>
        `,
        className: 'custom-vehicle-marker',
        iconSize: [40, 40],
        iconAnchor: [20, 20]
      });

      // Add marker with popup
      this.vehicleMarker = L.marker([lat, lng], { icon: vehicleIcon })
        .addTo(this.map)
        .bindPopup(`
          <div class="vehicle-popup">
            <h4>${this.vehicleId}</h4>
            <p><strong>Last Known Position</strong></p>
            <p>Lat: ${lat.toFixed(6)}</p>
            <p>Lng: ${lng.toFixed(6)}</p>
            ${this.vehicleStats?.lastKnownSpeed ? `<p>Speed: ${this.vehicleStats.lastKnownSpeed} km/h</p>` : ''}
            ${this.vehicleStats?.lastEventTime ? `<p>Updated: ${this.formatDateTime(this.vehicleStats.lastEventTime)}</p>` : ''}
          </div>
        `);

      // Center map on vehicle location
      this.map.setView([lat, lng], 15);
    }
  }

  private fixLeafletIcons(): void {
    // Fix for Leaflet default marker icons in Angular
    delete (L.Icon.Default.prototype as any)._getIconUrl;
    L.Icon.Default.mergeOptions({
      iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon-2x.png',
      iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-icon.png',
      shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.7.1/images/marker-shadow.png',
    });
  }
}