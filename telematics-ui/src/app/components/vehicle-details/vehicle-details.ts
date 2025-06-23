import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { BaseChartDirective } from 'ng2-charts';
import { Chart, ChartConfiguration, ChartData, ChartType, registerables } from 'chart.js';
import { VehicleService } from '../../services/vehicle.service';
import { Vehicle, VehicleStats, TelematicsEvent } from '../../models/vehicle.model';

Chart.register(...registerables);

@Component({
  selector: 'app-vehicle-details',
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatProgressSpinnerModule,
    BaseChartDirective
  ],
  templateUrl: './vehicle-details.html',
  styleUrl: './vehicle-details.scss'
})
export class VehicleDetails implements OnInit {
  vehicleId!: string;
  vehicle: Vehicle | null = null;
  vehicleStats: VehicleStats | null = null;
  events: TelematicsEvent[] = [];
  loading = true;

  // Chart configurations
  speedChartData: ChartData<'line'> = {
    labels: [],
    datasets: [{
      label: 'Speed (km/h)',
      data: [],
      borderColor: '#3f51b5',
      backgroundColor: 'rgba(63, 81, 181, 0.1)',
      tension: 0.1
    }]
  };

  speedChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        display: true,
      },
      title: {
        display: true,
        text: 'Speed Over Time'
      }
    },
    scales: {
      x: {
        display: true,
        title: {
          display: true,
          text: 'Time'
        }
      },
      y: {
        display: true,
        title: {
          display: true,
          text: 'Speed (km/h)'
        }
      }
    }
  };

  eventTypeChartData: ChartData<'doughnut'> = {
    labels: [],
    datasets: [{
      data: [],
      backgroundColor: [
        '#FF6384',
        '#36A2EB',
        '#FFCE56',
        '#4BC0C0',
        '#9966FF',
        '#FF9F40'
      ]
    }]
  };

  eventTypeChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: {
      legend: {
        position: 'right',
      },
      title: {
        display: true,
        text: 'Event Types Distribution'
      }
    }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private vehicleService: VehicleService
  ) {}

  ngOnInit(): void {
    this.vehicleId = this.route.snapshot.paramMap.get('id') || '';
    if (this.vehicleId) {
      this.loadVehicleData();
    }
  }

  loadVehicleData(): void {
    this.loading = true;
    
    // Load vehicle stats
    this.vehicleService.getVehicleStats(this.vehicleId).subscribe({
      next: (stats) => {
        this.vehicleStats = stats;
      },
      error: (error) => {
        console.error('Error loading vehicle stats:', error);
      }
    });

    // Load vehicle events for the last 30 days
    const endTime = new Date();
    const startTime = new Date();
    startTime.setDate(startTime.getDate() - 30);

    this.vehicleService.getVehicleEvents(
      this.vehicleId, 
      startTime.toISOString(), 
      endTime.toISOString()
    ).subscribe({
      next: (events) => {
        this.events = events;
        this.updateCharts();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading vehicle events:', error);
        this.loading = false;
      }
    });
  }

  updateCharts(): void {
    this.updateSpeedChart();
    this.updateEventTypeChart();
  }

  updateSpeedChart(): void {
    const speedEvents = this.events.filter(e => e.speed !== null && e.speed !== undefined);
    const last24Hours = speedEvents.slice(-24); // Last 24 events for speed chart

    this.speedChartData = {
      labels: last24Hours.map(e => new Date(e.timestamp).toLocaleTimeString()),
      datasets: [{
        label: 'Speed (km/h)',
        data: last24Hours.map(e => e.speed || 0),
        borderColor: '#3f51b5',
        backgroundColor: 'rgba(63, 81, 181, 0.1)',
        tension: 0.1
      }]
    };
  }

  updateEventTypeChart(): void {
    const eventTypeCounts: { [key: string]: number } = {};
    
    this.events.forEach(event => {
      eventTypeCounts[event.eventType] = (eventTypeCounts[event.eventType] || 0) + 1;
    });

    this.eventTypeChartData = {
      labels: Object.keys(eventTypeCounts),
      datasets: [{
        data: Object.values(eventTypeCounts),
        backgroundColor: [
          '#FF6384',
          '#36A2EB',
          '#FFCE56',
          '#4BC0C0',
          '#9966FF',
          '#FF9F40'
        ]
      }]
    };
  }

  goBack(): void {
    this.router.navigate(['/dashboard']);
  }

  formatDateTime(dateString?: string): string {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString();
  }
}
