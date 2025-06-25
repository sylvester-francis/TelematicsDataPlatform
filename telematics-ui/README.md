# TelematicsHQ Frontend - Premium Angular Dashboard

A **professional, enterprise-grade Angular dashboard** featuring interactive maps, real-time analytics, and glass morphism design for the TelematicsHQ fleet management platform.

![Angular](https://img.shields.io/badge/Angular-18-red) ![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue) ![Leaflet](https://img.shields.io/badge/Leaflet-Maps-green) ![Material](https://img.shields.io/badge/Material-Design-blue)

## 🚀 Features Overview

### **🎨 Premium UI/UX Design**
- **Glass Morphism**: Modern translucent card effects with backdrop blur
- **Responsive Layout**: Mobile-first design with professional animations
- **Material Design**: Custom-themed Angular Material components
- **Professional Typography**: Inter font family with perfect hierarchy
- **Consistent Spacing**: 8px grid system for perfect alignment

### **🗺️ Interactive Mapping**
- **Real-time Vehicle Tracking**: Live location display with Leaflet.js
- **Custom Vehicle Markers**: Car emoji markers with animated pulse effects
- **Detailed Popups**: Vehicle information, coordinates, speed, and timestamps
- **Street-level Detail**: Zoom level 15 for precise location viewing
- **Mobile Responsive**: Optimized map experience on all devices

### **📊 Real-time Analytics**
- **Live System Metrics**: Total vehicles, events, alerts from backend APIs
- **Vehicle Statistics**: Performance data including events, trips, and speed
- **Interactive Fleet Table**: Professional data tables with actions
- **Health Monitoring**: Real-time API health checks and status

### **🚗 Fleet Management**
- **Vehicle Dashboard**: Comprehensive fleet overview with real-time data
- **Vehicle Details**: Individual vehicle analytics with location maps
- **Event Submission**: Professional forms for telemetry data entry
- **Event History**: Timeline view of vehicle events with status indicators

## 🏗️ Architecture

### **Component Structure**
```
src/app/
├── components/
│   ├── nav-toolbar/           # Professional navigation header
│   ├── vehicle-dashboard/     # Fleet overview with real-time metrics
│   ├── vehicle-details/       # Individual vehicle analytics + maps
│   └── event-form/           # Telemetry data submission forms
├── services/
│   └── vehicle.service.ts    # Backend API integration
├── models/
│   └── vehicle.model.ts      # TypeScript interfaces
└── styles/
    └── professional design system with CSS custom properties
```

### **Design System**
```scss
:root {
  // Professional color palette
  --primary-500: #3b82f6;     // Primary blue
  --gray-900: #111827;        // Dark text
  --space-4: 1rem;            // Consistent spacing
  --radius-lg: 0.5rem;        // Card border radius
  --shadow-xl: ...;           // Professional shadows
}
```

## 🎯 Quick Start

### **Development Setup**
```bash
# Install dependencies
npm install

# Start development server
npm start

# Access the application
open http://localhost:4201
```

### **Backend Integration**
```typescript
// Configure API endpoint in vehicle.service.ts
private apiUrl = 'http://localhost:5000/api';

// Real backend endpoints
GET /api/vehicles                    # Fleet overview
GET /api/vehicles/{id}/stats        # Vehicle statistics
GET /api/telematics/vehicles/{id}/events  # Event history
POST /api/telematics/events         # Submit events
GET /api/health/metrics             # System metrics
```

## 📱 Component Features

### **Vehicle Dashboard**
- **Real-time Metrics**: System statistics from `/api/health/metrics`
- **Fleet Overview**: Vehicle list with performance data
- **Quick Actions**: Event submission and system health shortcuts
- **Professional Layout**: Glass morphism cards with smooth animations

### **Vehicle Details with Maps**
- **Interactive Maps**: Street-level vehicle location tracking
- **Vehicle Statistics**: Events, trips, alerts, and speed metrics
- **Event Timeline**: Professional event history with status badges
- **Mobile Responsive**: Optimized for all screen sizes

### **Event Form**
- **Real Backend Integration**: Submits to `/api/telematics/events`
- **Professional Validation**: Form validation with error handling
- **Sample Data**: Quick-fill functionality for testing
- **Success Feedback**: Clear confirmation with event IDs

### **Navigation**
- **Professional Branding**: TelematicsHQ logo and styling
- **Mobile Navigation**: Responsive slide-out menu
- **Quick Access**: Dashboard and event submission shortcuts

## 🗺️ Mapping Integration

### **Leaflet.js Setup**
```typescript
// Map initialization in vehicle-details.component.ts
private initializeMap(): void {
  this.map = L.map(this.mapContainer.nativeElement, {
    center: [lat, lng],
    zoom: 15,
    zoomControl: true,
    scrollWheelZoom: true
  });

  // OpenStreetMap tiles
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors',
    maxZoom: 19
  }).addTo(this.map);
}
```

### **Custom Vehicle Markers**
```typescript
// Custom car marker with pulse animation
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
```

### **Map Styling**
```scss
.map-container {
  height: 400px;
  width: 100%;
  
  @media (max-width: 768px) {
    height: 300px;
  }
}

.custom-vehicle-marker {
  .marker-pulse {
    animation: pulse 2s infinite;
    background: rgba(59, 130, 246, 0.3);
    border-radius: 50%;
  }
}
```

## 🎨 Design System

### **Glass Morphism Effects**
```scss
.glass-card {
  background: rgba(255, 255, 255, 0.25);
  backdrop-filter: blur(10px);
  border: 1px solid rgba(255, 255, 255, 0.18);
  box-shadow: 0 8px 32px rgba(31, 38, 135, 0.37);
  border-radius: var(--radius-xl);
}
```

### **Professional Animations**
```scss
.animate-fade-in-up {
  animation: fadeInUp 0.6s ease-out;
}

.animate-slide-in-right {
  animation: slideInRight 0.8s ease-out;
}

@keyframes fadeInUp {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}
```

### **Responsive Design**
```scss
// Mobile-first approach
.dashboard-layout {
  display: grid;
  gap: var(--space-6);
  
  @media (min-width: 768px) {
    grid-template-columns: repeat(2, 1fr);
  }
  
  @media (min-width: 1200px) {
    grid-template-columns: repeat(3, 1fr);
  }
}
```

## 🔧 Backend Integration

### **Real API Calls**
```typescript
// No mock data - all real backend integration
export class VehicleService {
  getVehicles(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(`${this.apiUrl}/vehicles`);
  }

  getVehicleStats(id: string): Observable<VehicleStats> {
    return this.http.get<VehicleStats>(`${this.apiUrl}/vehicles/${id}/stats`);
  }

  getSystemMetrics(): Observable<any> {
    return this.http.get(`${this.apiUrl}/health/metrics`);
  }
}
```

### **Modern RxJS Usage**
```typescript
// Using firstValueFrom instead of deprecated toPromise()
const response = await firstValueFrom(this.vehicleService.submitEvent(payload));
```

### **Error Handling**
```typescript
// Professional error handling with user feedback
catch (error: any) {
  if (error.status === 400) {
    this.submitError = 'Invalid data provided. Please check your inputs.';
  } else if (error.status === 500) {
    this.submitError = 'Server error occurred. Please try again later.';
  }
}
```

## 📊 Technology Stack

### **Core Technologies**
- **Angular 18**: Latest stable version with standalone components
- **TypeScript 5.0**: Strict mode for type safety
- **Angular Material**: Custom-themed UI components
- **Leaflet.js**: Interactive mapping library
- **SCSS**: Advanced styling with CSS custom properties
- **RxJS**: Reactive programming for HTTP calls

### **Development Tools**
- **Angular CLI**: Project scaffolding and build tools
- **Vite**: Fast development server and build tool
- **ESLint**: Code linting and quality checks
- **Prettier**: Code formatting
- **TypeScript**: Strict type checking

### **Dependencies**
```json
{
  "@angular/core": "^18.0.0",
  "@angular/material": "^18.0.0",
  "leaflet": "^1.9.4",
  "@types/leaflet": "^1.9.8",
  "rxjs": "^7.8.0"
}
```

## 🚀 Build & Deployment

### **Development**
```bash
npm start           # Development server
npm test            # Unit tests
npm run lint        # Code linting
npm run build       # Production build
```

### **Production Build**
```bash
# Build for production
npm run build

# Output in dist/ folder
# Optimized bundles with tree-shaking
# Minified CSS and JavaScript
# Service worker ready
```

### **Docker Deployment**
```dockerfile
# Multi-stage build for production
FROM node:18-alpine as build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist/ /usr/share/nginx/html/
EXPOSE 80
```

## 🎯 Key Features Summary

### **Real Backend Integration**
- ✅ Live system metrics from `/api/health/metrics`
- ✅ Vehicle statistics from `/api/vehicles/{id}/stats`
- ✅ Event submission to `/api/telematics/events`
- ✅ Proper error handling and validation

### **Interactive Mapping**
- ✅ Real GPS coordinates from backend
- ✅ Custom vehicle markers with animations
- ✅ Mobile-responsive map experience
- ✅ Professional popup information

### **Professional UI/UX**
- ✅ Glass morphism design system
- ✅ Responsive mobile-first layout
- ✅ Professional animations and transitions
- ✅ Consistent spacing and typography

### **Enterprise Features**
- ✅ TypeScript strict mode
- ✅ Component-based architecture
- ✅ Real-time data updates
- ✅ Professional error handling

## 📱 Mobile Experience

### **Responsive Design**
- **Breakpoints**: 640px, 768px, 1024px, 1200px
- **Mobile Navigation**: Slide-out menu with touch gestures
- **Touch-friendly**: Large touch targets and optimized interactions
- **Map Experience**: Mobile-optimized Leaflet controls

### **Performance**
- **Lazy Loading**: Components loaded on demand
- **Optimized Bundles**: Tree-shaking and code splitting
- **Fast Loading**: Optimized assets and caching
- **Smooth Animations**: Hardware-accelerated transitions

---

**TelematicsHQ Frontend** - *Professional Angular Dashboard with Interactive Maps*
Built with ❤️ using Angular, TypeScript, Leaflet, and modern web technologies.