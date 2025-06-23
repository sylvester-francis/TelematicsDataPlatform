# Telematics Data Platform

A production-ready C# application demonstrating telematics data processing using Entity Framework, built for interview preparation.

## Quick Start

1. **Setup Database**: `./setup-database.sh`
2. **Run API**: `./run-api.sh`
3. **Run Batch Processor**: `./run-batch.sh` (in another terminal)
4. **Run Tests**: `./run-tests.sh`

## Docker

- **Run with Docker**: `./run-docker.sh`

## Endpoints

- API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- Health: http://localhost:5000/api/health

## Project Structure

- **TelematicsCore**: Business logic, models, services
- **TelematicsData**: Entity Framework DbContext
- **TelematicsApi**: REST API controllers
- **TelematicsBatchProcessor**: Background processing service
- **TelematicsTests**: Unit and integration tests

## Key Features

- Real-time telematics data ingestion
- Batch processing with enrichment
- Geospatial data support
- Alert generation
- Health monitoring
- Docker containerization
