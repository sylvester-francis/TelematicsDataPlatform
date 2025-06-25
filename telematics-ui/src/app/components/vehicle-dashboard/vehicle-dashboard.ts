import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { VehicleService } from '../../services/vehicle.service';
import { Vehicle, VehicleStats } from '../../models/vehicle.model';

@Component({
  selector: 'app-vehicle-dashboard',
  imports: [
    CommonModule,
    MatCardModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
  templateUrl: './vehicle-dashboard.html',
  styleUrl: './vehicle-dashboard.scss'
})
export class VehicleDashboard implements OnInit {
  vehicles: Vehicle[] = [];
  vehicleStats: { [key: string]: VehicleStats } = {};
  systemMetrics: {
    totalVehicles: number;
    totalEvents: number;
    unprocessedEvents: number;
    totalAlerts: number;
    timestamp: string;
  } | null = null;
  backendSupportedColumns: string[] = ['vehicleId', 'vehicleInfo', 'stats', 'lastEvent', 'actions'];
  loading = true;

  constructor(
    private vehicleService: VehicleService,
    public router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadSystemMetrics();
    this.loadVehicles();
  }

  loadVehicles(): void {
    this.loading = true;
    this.vehicleService.getVehicles().subscribe({
      next: (vehicles) => {
        this.vehicles = vehicles;
        vehicles.forEach(vehicle => {
          this.loadVehicleStats(vehicle.vehicleIdentifier);
        });
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading vehicles:', error);
        this.handleError('Failed to load vehicles. Please try again later.');
        this.loading = false;
      }
    });
  }

  loadVehicleStats(vehicleIdentifier: string): void {
    this.vehicleService.getVehicleStats(vehicleIdentifier).subscribe({
      next: (stats) => {
        this.vehicleStats[vehicleIdentifier] = stats;
      },
      error: (error) => {
        console.error(`Error loading stats for vehicle ${vehicleIdentifier}:`, error);
      }
    });
  }

  getStats(vehicleIdentifier: string): VehicleStats | null {
    return this.vehicleStats[vehicleIdentifier] || null;
  }

  viewVehicleDetails(vehicleIdentifier: string): void {
    this.router.navigate(['/vehicle', vehicleIdentifier]);
  }

  formatDateTime(dateString?: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString();
  }

  handleError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      panelClass: ['error-snackbar'],
      horizontalPosition: 'center',
      verticalPosition: 'top'
    });
  }

  retryLoadVehicles(): void {
    this.loadVehicles();
  }

  refreshData(): void {
    this.loadVehicles();
  }

  loadSystemMetrics(): void {
    this.vehicleService.getSystemMetrics().subscribe({
      next: (metrics: any) => {
        this.systemMetrics = metrics;
      },
      error: (error: any) => {
        console.error('Error loading system metrics:', error);
      }
    });
  }

  checkSystemHealth(): void {
    this.vehicleService.getSystemHealth().subscribe({
      next: (health: any) => {
        const message = `System Status: ${health.status} | Database: ${health.database} | Version: ${health.version}`;
        this.snackBar.open(message, 'Close', {
          duration: 5000,
          panelClass: health.status === 'Healthy' ? ['success-snackbar'] : ['error-snackbar']
        });
      },
      error: (error: any) => {
        this.handleError('Failed to check system health');
      }
    });
  }

  formatDate(dateString?: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString();
  }

  navigateToSubmitEvent(): void {
    this.router.navigate(['/submit-event']);
  }
}
