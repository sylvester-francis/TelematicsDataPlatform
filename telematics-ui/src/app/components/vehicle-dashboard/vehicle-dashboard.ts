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
  displayedColumns: string[] = ['vehicleIdentifier', 'make', 'model', 'year', 'events', 'lastActivity', 'actions'];
  enhancedDisplayedColumns: string[] = ['status', 'vehicle', 'metrics', 'lastActivity', 'actions'];
  loading = true;

  constructor(
    private vehicleService: VehicleService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
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

  getTotalEvents(): number {
    return Object.values(this.vehicleStats).reduce((total, stats) => total + (stats.totalEvents || 0), 0);
  }

  getTotalAlerts(): number {
    return Object.values(this.vehicleStats).reduce((total, stats) => total + (stats.totalAlerts || 0), 0);
  }
}
