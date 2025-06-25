import { Routes } from '@angular/router';
import { VehicleDashboard } from './components/vehicle-dashboard/vehicle-dashboard';
import { VehicleDetails } from './components/vehicle-details/vehicle-details';
import { EventForm } from './components/event-form/event-form';

export const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: VehicleDashboard },
  { path: 'vehicle/:id', component: VehicleDetails },
  { path: 'submit-event', component: EventForm },
  { path: '**', redirectTo: '/dashboard' }
];
