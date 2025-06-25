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
    maintainAspectRatio: false,
    plugins: {
      legend: {
        display: true,
        position: 'top'
      },
      title: {
        display: true,
        text: 'Speed Over Time',
        font: {
          size: 16,
          weight: 'bold'
        }
      },
      tooltip: {
        backgroundColor: 'rgba(0, 0, 0, 0.8)',
        titleColor: 'white',
        bodyColor: 'white',
        borderColor: '#667eea',
        borderWidth: 1
      }
    },
    scales: {
      x: {
        display: true,
        title: {
          display: true,
          text: 'Time',
          font: {
            weight: 'bold'
          }
        },
        grid: {
          color: 'rgba(0, 0, 0, 0.1)'
        }
      },
      y: {
        display: true,
        title: {
          display: true,
          text: 'Speed (km/h)',
          font: {
            weight: 'bold'
          }
        },
        grid: {
          color: 'rgba(0, 0, 0, 0.1)'
        },
        beginAtZero: true
      }
    },
    animation: {
      duration: 1000,
      easing: 'easeInOutQuart'
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
    maintainAspectRatio: false,
    plugins: {
      legend: {
        position: 'right',
        labels: {
          padding: 20,
          usePointStyle: true,
          font: {
            size: 12
          }
        }
      },
      title: {
        display: true,
        text: 'Event Types Distribution',
        font: {
          size: 16,
          weight: 'bold'
        },
        padding: 20
      },
      tooltip: {
        backgroundColor: 'rgba(0, 0, 0, 0.8)',
        titleColor: 'white',
        bodyColor: 'white',
        borderColor: '#667eea',
        borderWidth: 1,
        callbacks: {
          label: function(context) {
            const total = context.dataset.data.reduce((a: any, b: any) => a + b, 0);
            const percentage = ((context.parsed / total) * 100).toFixed(1);
            return `${context.label}: ${context.parsed} (${percentage}%)`;
          }
        }
      }
    },
    animation: {
      duration: 1500
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
        borderColor: '#667eea',
        backgroundColor: 'rgba(102, 126, 234, 0.1)',
        pointBackgroundColor: '#667eea',
        pointBorderColor: '#ffffff',
        pointBorderWidth: 2,
        pointRadius: 4,
        pointHoverRadius: 6,
        fill: true,
        tension: 0.4
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
          '#667eea',
          '#764ba2',
          '#f093fb',
          '#f5576c',
          '#4facfe',
          '#00f2fe',
          '#43e97b',
          '#38f9d7',
          '#ffecd2',
          '#fcb69f'
        ],
        borderWidth: 2,
        borderColor: '#ffffff',
        hoverBorderWidth: 3,
        hoverBorderColor: '#333333'
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

  getEventTypeClass(eventType: string): string {
    const typeMap: { [key: string]: string } = {
      'POSITION': 'position',
      'IGNITION_ON': 'ignition-on',
      'IGNITION_OFF': 'ignition-off',
      'SPEEDING': 'speeding',
      'HARSH_BRAKING': 'harsh-braking',
      'HARSH_ACCELERATION': 'harsh-acceleration'
    };
    return typeMap[eventType] || 'default';
  }

  getEventIcon(eventType: string): string {
    const iconMap: { [key: string]: string } = {
      'POSITION': 'location_on',
      'IGNITION_ON': 'power_settings_new',
      'IGNITION_OFF': 'power_off',
      'SPEEDING': 'speed',
      'HARSH_BRAKING': 'warning',
      'HARSH_ACCELERATION': 'trending_up'
    };
    return iconMap[eventType] || 'info';
  }
}
