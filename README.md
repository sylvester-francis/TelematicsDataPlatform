# TelematicsHQ - Enterprise Fleet Management Platform

A **world-class, enterprise-grade telematics platform** featuring a premium Angular dashboard with interactive maps and robust .NET backend. Built with professional design principles and enterprise-level architecture.

[![Build Status](https://github.com/sylvester-francis/TelematicsDataPlatform/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/sylvester-francis/TelematicsDataPlatform/actions) ![TelematicsHQ Preview](https://img.shields.io/badge/Status-Production%20Ready-brightgreen) ![Angular](https://img.shields.io/badge/Angular-18-red) ![.NET](https://img.shields.io/badge/.NET-Core-blue) ![TypeScript](https://img.shields.io/badge/TypeScript-5.0-blue) ![Docker](https://img.shields.io/badge/Docker-Full%20Stack-blue)

## 🚀 Platform Overview

**TelematicsHQ** is a comprehensive fleet management solution that combines real-time vehicle tracking, interactive mapping, advanced analytics, and professional data visualization. The platform features a stunning Angular frontend with glass morphism design and a scalable .NET backend with Entity Framework.

### ✨ **Key Highlights**
- 🎨 **Premium UI/UX**: Glass morphism design with smooth animations
- 🗺️ **Interactive Maps**: Real-time vehicle location tracking with Leaflet integration
- 📊 **Real-time Analytics**: Live system metrics and fleet performance dashboards
- 🚗 **Fleet Management**: Vehicle tracking, event processing, and alert systems
- 📱 **Mobile-First**: Responsive design for all devices
- 🔧 **Enterprise-Ready**: Scalable architecture with Docker support
- ⚡ **CI/CD Pipeline**: Automated testing and deployment with GitHub Actions

## 🎯 Quick Start Guide

### **Option 1: Full Stack Development**

1. **Start the Backend API**
   ```bash
   # Setup database (SQL Server in Docker)
   docker-compose up sql-server -d
   
   # Run API server
   cd TelematicsApi
   dotnet run --urls="http://localhost:5000"
   ```

2. **Start the Premium Frontend**
   ```bash
   cd telematics-ui
   npm install
   npm start
   ```

3. **Access the Platform**
   - 🌐 **Dashboard**: http://localhost:4201
   - 🔧 **API**: http://localhost:5000
   - 📖 **API Docs**: http://localhost:5000/swagger

### **Option 2: Docker Deployment**
```bash
# Run full-stack container (Frontend + Backend + Database)
docker-compose up telematics-fullstack sql-server -d

# Or run separate services
docker-compose up -d
```

### **Option 3: GitHub Container Registry**
```bash
# Pull and run from GitHub registry
docker run -d -p 80:80 -p 5000:5000 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal,1433;Database=TelematicsDB;User Id=sa;Password=YourPass;TrustServerCertificate=true" \
  ghcr.io/sylvester-francis/telematicsdataplatform/telematics-fullstack:latest
```

## 🗺️ Interactive Map Features

### **Real-time Vehicle Tracking**
- **Live Location Display**: Interactive maps showing exact vehicle positions
- **Custom Vehicle Markers**: Car emoji markers with animated pulse effects
- **Detailed Popups**: Click markers to see vehicle information, speed, and last update
- **Street-level Detail**: Zoom level 15 for precise location viewing
- **OpenStreetMap Integration**: High-quality, free map tiles

### **Location Intelligence**
- **GPS Coordinates**: Latitude/longitude display with 6-decimal precision
- **Last Known Position**: Real-time updates from backend telemetry data
- **Speed Information**: Current/last known vehicle speed
- **Timestamp Tracking**: Precise last update times

## 🏗️ Architecture Overview

### **Frontend - Premium Angular Dashboard**
```
telematics-ui/
├── 🎨 Professional Design System
│   ├── Glass morphism effects
│   ├── Advanced animations
│   ├── Responsive grid layouts
│   └── Premium color schemes
├── 📊 Advanced Components
│   ├── Vehicle Dashboard (Real-time fleet metrics)
│   ├── Vehicle Details (Analytics & interactive maps)
│   ├── Event Forms (Telemetry data submission)
│   └── Navigation (Mobile-responsive)
├── 🗺️ Mapping Integration
│   ├── Leaflet.js interactive maps
│   ├── Custom vehicle markers
│   ├── Real-time location updates
│   └── Professional map styling
└── 🚀 Modern Tech Stack
    ├── Angular 18 + Material Design
    ├── Leaflet for mapping
    ├── SCSS with CSS custom properties
    └── TypeScript with strict mode
```

### **Backend - Enterprise .NET API**
```
TelematicsDataPlatform/
├── 🏢 TelematicsCore/
│   ├── Business logic & domain models
│   ├── Service interfaces & implementations
│   └── Geospatial data processing
├── 💾 TelematicsData/
│   ├── Entity Framework DbContext
│   ├── Database migrations
│   └── NetTopologySuite geospatial support
├── 🌐 TelematicsApi/
│   ├── REST API controllers
│   ├── Health monitoring endpoints
│   └── Vehicle telemetry endpoints
├── ⚙️ TelematicsBatchProcessor/
│   ├── Background data processing
│   ├── Event enrichment pipeline
│   └── Alert generation system
└── 🧪 TelematicsTests/
    ├── Unit tests
    ├── Integration tests
    └── API endpoint testing
```

## 🎨 Premium UI Features

### **Dashboard Excellence**
- **Real-time Metrics**: Live system statistics from backend APIs
- **Glass Morphism**: Translucent cards with backdrop blur effects
- **Interactive Fleet Table**: Vehicle list with statistics and actions
- **System Health**: Real-time API health monitoring
- **Responsive Design**: Mobile-optimized layouts

### **Vehicle Analytics**
- **Interactive Maps**: Street-level vehicle location tracking
- **Performance Metrics**: Total events, trips, alerts, and speed data
- **Event Timeline**: Professional event history with status indicators
- **Location Intelligence**: GPS coordinates and last update times
- **Real-time Updates**: Live telemetry data from backend

### **Professional Navigation**
- **TelematicsHQ Branding**: Clean, professional header design
- **Mobile Navigation**: Responsive slide-out menu
- **Quick Actions**: System health and event submission shortcuts

## 🔧 API Endpoints

### **Core Backend Integration**
```
GET    /api/vehicles                    # Fleet overview
GET    /api/vehicles/{id}/stats        # Vehicle performance metrics
GET    /api/telematics/vehicles/{id}/events  # Event history
POST   /api/telematics/events          # Submit single event
POST   /api/telematics/events/batch    # Batch event submission
GET    /api/health                     # System health check
GET    /api/health/metrics             # System metrics dashboard
```

### **Real Data Features**
- **No Mock Data**: All UI data comes from actual backend APIs
- **Real-time Updates**: Live system metrics and vehicle statistics
- **Backend Validation**: Proper error handling and form validation
- **Geospatial Support**: NetTopologySuite for location data processing

## 🚀 Enterprise Features

### **Production-Ready Architecture**
- **Entity Framework Core**: Optimized database operations with migrations
- **Structured Logging**: Serilog with file and console output
- **Health Monitoring**: Comprehensive API health checks
- **Error Handling**: Graceful error responses and user feedback
- **Docker Support**: Containerized deployment ready

### **Development Experience**
- **Hot Reload**: Instant development feedback for both frontend and backend
- **Type Safety**: Full TypeScript coverage with strict mode
- **Testing**: Comprehensive API integration tests
- **CI/CD Pipeline**: Automated testing and deployment

## 📊 Technology Stack

### **Frontend Technologies**
- **Framework**: Angular 18 with standalone components
- **UI Library**: Angular Material with custom glass morphism theming
- **Mapping**: Leaflet.js for interactive maps and geospatial visualization
- **HTTP Client**: Modern RxJS with firstValueFrom for API calls
- **Styling**: SCSS with CSS custom properties and responsive design
- **Icons**: Material Design icons
- **Fonts**: Inter typeface with multiple weights

### **Backend Technologies**
- **Runtime**: .NET Core with C#
- **Database**: SQL Server with NetTopologySuite spatial extensions
- **ORM**: Entity Framework Core with code-first migrations
- **Logging**: Serilog with structured logging to files and console
- **Testing**: xUnit with integration testing
- **Containerization**: Docker with SQL Server container support

### **DevOps & CI/CD**
- **Version Control**: Git with GitHub
- **CI/CD**: GitHub Actions for automated testing and deployment
- **Containerization**: Docker with multi-stage builds
- **Database**: SQL Server in Docker for development

## 🚀 CI/CD Pipeline

### **GitHub Actions Workflow**
[![CI/CD Pipeline](https://github.com/sylvester-francis/TelematicsDataPlatform/actions/workflows/ci-cd.yml/badge.svg)](https://github.com/sylvester-francis/TelematicsDataPlatform/actions)

**Automated Testing & Deployment**
- ✅ **Backend Tests**: .NET unit tests with SQL Server containers
- ✅ **Frontend Tests**: Angular unit tests and linting
- ✅ **Integration Tests**: Full API testing with database
- ✅ **Security Scanning**: Trivy vulnerability scanning
- ✅ **Code Quality**: SonarCloud analysis
- ✅ **Docker Builds**: Multi-platform container builds

### **Container Registry**
```bash
# GitHub Container Registry (GHCR)
ghcr.io/sylvester-francis/telematicsdataplatform/telematics-fullstack:latest

# Automatic builds on main branch
# Multi-platform: linux/amd64, linux/arm64
# Health checks and deployment manifests included
```

### **Deployment Environments**
- **Development**: Local Docker with hot reload
- **Staging**: Automated deployment from `develop` branch
- **Production**: Automated deployment from `main` branch
- **Performance Testing**: Automated load testing on releases

## 🎯 Getting Started for Development

### **Prerequisites**
- Node.js 18+ (for Angular frontend)
- .NET Core SDK (for backend development)
- Docker (for SQL Server database)
- Git (for version control)

### **Development Workflow**

1. **Clone & Setup**
   ```bash
   git clone https://github.com/sylvester-francis/TelematicsDataPlatform.git
   cd TelematicsDataPlatform
   ```

2. **Backend Development**
   ```bash
   # Start SQL Server in Docker
   docker-compose up sql-server -d
   
   # Run API with hot reload
   cd TelematicsApi
   dotnet watch run --urls="http://localhost:5000"
   ```

3. **Frontend Development**
   ```bash
   # Install dependencies
   cd telematics-ui
   npm install
   
   # Start development server
   npm start
   ```

4. **Testing**
   ```bash
   # Run backend tests
   cd TelematicsTests
   dotnet test
   
   # Run frontend tests
   cd telematics-ui
   npm test
   ```

## 🌟 Key Features Showcase

### **Real Backend Integration**
- **Live System Metrics**: Total vehicles, events, alerts from `/api/health/metrics`
- **Vehicle Statistics**: Real performance data from `/api/vehicles/{id}/stats`
- **Event Submission**: Actual form submission to `/api/telematics/events`
- **Error Handling**: Proper validation and user feedback

### **Interactive Mapping**
- **Real Location Data**: Maps display actual GPS coordinates from backend
- **Custom Markers**: Professional car emoji markers with pulse animations
- **Detailed Popups**: Vehicle information, coordinates, speed, and timestamps
- **Mobile Responsive**: Optimized map experience on all devices

### **Professional UI/UX**
- **Glass Morphism Design**: Modern translucent card effects
- **Responsive Layout**: Mobile-first design with professional animations
- **Real-time Updates**: Live data refresh and status indicators
- **No Mock Data**: Everything connected to actual backend APIs

## 📈 Production Deployment

### **Docker Production Setup**
```bash
# Full-stack deployment (recommended)
docker-compose up telematics-fullstack sql-server -d

# From GitHub Container Registry
docker pull ghcr.io/sylvester-francis/telematicsdataplatform/telematics-fullstack:latest

# Separate services (alternative)
docker-compose up -d

# Build production images locally
docker build -f Dockerfile.fullstack -t telematics-fullstack .
docker build -t telematics-api ./TelematicsApi
docker build -t telematics-ui ./telematics-ui
```

### **Environment Configuration**
- **Development**: Local development with Docker SQL Server
- **Staging**: Docker containers with external database
- **Production**: Kubernetes deployment ready with CI/CD pipeline

## 🤝 Contributing

We welcome contributions to make TelematicsHQ even better!

### **Contribution Process**
1. **Fork** the [repository](https://github.com/sylvester-francis/TelematicsDataPlatform)
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Make** your changes and add tests
4. **Run** tests locally: `dotnet test && cd telematics-ui && npm test`
5. **Commit** your changes (`git commit -m 'Add amazing feature'`)
6. **Push** to your branch (`git push origin feature/amazing-feature`)
7. **Open** a Pull Request

### **Development Guidelines**
- ✅ All tests must pass (CI will verify)
- ✅ Follow existing code patterns and styles
- ✅ Add tests for new features
- ✅ Update documentation if needed
- ✅ Ensure Docker builds successfully

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**TelematicsHQ** - *Professional Fleet Management Platform with Interactive Maps*

🔗 **Repository**: [https://github.com/sylvester-francis/TelematicsDataPlatform](https://github.com/sylvester-francis/TelematicsDataPlatform)
📦 **Container Registry**: [ghcr.io/sylvester-francis/telematicsdataplatform](https://github.com/sylvester-francis/TelematicsDataPlatform/pkgs/container/telematicsdataplatform%2Ftelematics-fullstack)
🚀 **CI/CD**: [GitHub Actions](https://github.com/sylvester-francis/TelematicsDataPlatform/actions)

Built with ❤️ using Angular, .NET Core, Leaflet, Docker, and modern web technologies.