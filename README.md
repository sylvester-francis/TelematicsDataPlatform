# TelematicsHQ - Fleet Management Platform

[![Build Status](https://github.com/sylvester-francis/TelematicsDataPlatform/actions/workflows/simple-ci-cd.yml/badge.svg)](https://github.com/sylvester-francis/TelematicsDataPlatform/actions) ![Angular](https://img.shields.io/badge/Angular-18-red) ![.NET](https://img.shields.io/badge/.NET-9.0-blue) ![Docker](https://img.shields.io/badge/Docker-Ready-blue)

A modern fleet management platform built with Angular 18 and .NET 9.0, featuring interactive maps and real-time vehicle tracking.

## Features

- **Interactive Dashboard** - Real-time fleet metrics and vehicle status
- **Vehicle Tracking** - GPS location mapping with Leaflet.js
- **Event Management** - Submit and track telemetry events
- **Health Monitoring** - API health checks and system metrics
- **Responsive Design** - Mobile-friendly interface

## Quick Start

### Docker (Recommended)
```bash
# Run full stack with database
docker-compose up -d

# Access the application
# Frontend: http://localhost:4201
# Backend API: http://localhost:5000
# Swagger: http://localhost:5000/swagger
```

### Development Setup
```bash
# Backend
cd TelematicsApi
dotnet run --urls="http://localhost:5000"

# Frontend
cd telematics-ui
npm install
npm start
```

## Architecture

### Frontend (Angular 18)
- Standalone components
- Angular Material UI
- Leaflet maps integration
- TypeScript strict mode
- Responsive design

### Backend (.NET 9.0)
- Web API with Entity Framework Core
- SQL Server with spatial data support
- Structured logging with Serilog
- Health checks and metrics
- Swagger documentation

## API Endpoints

```
GET    /api/vehicles                    # List all vehicles
GET    /api/vehicles/{id}/stats        # Vehicle statistics
GET    /api/telematics/vehicles/{id}/events  # Vehicle events
POST   /api/telematics/events          # Submit event
GET    /api/health                     # Health check
GET    /api/health/metrics             # System metrics
```

## Technology Stack

**Frontend:**
- Angular 18
- TypeScript 5.0
- Angular Material
- Leaflet.js
- SCSS

**Backend:**
- .NET 9.0
- Entity Framework Core
- SQL Server
- NetTopologySuite
- Serilog

**DevOps:**
- Docker & Docker Compose
- GitHub Actions CI/CD
- GitHub Container Registry

## Development

### Prerequisites
- Node.js 20.19+
- .NET SDK 9.0+
- Docker

### Running Tests
```bash
# Backend tests
dotnet test

# Frontend tests
cd telematics-ui
npm test
```

### Building
```bash
# Backend
dotnet build

# Frontend
cd telematics-ui
npm run build
```

## CI/CD

The project uses GitHub Actions for automated testing and deployment:
- Backend unit tests
- Frontend tests and builds
- Docker image builds
- Security scanning

Container images are published to GitHub Container Registry.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make changes and add tests
4. Run tests locally
5. Submit a pull request

## License

MIT License - see [LICENSE](LICENSE) file for details.

---

**Repository**: [https://github.com/sylvester-francis/TelematicsDataPlatform](https://github.com/sylvester-francis/TelematicsDataPlatform)