#!/bin/bash

# Telematics Data Platform Setup Script for JetBrains Rider
# This script sets up the complete project structure, dependencies, and initial configuration

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
print_status "Checking prerequisites..."

if ! command_exists dotnet; then
    print_error ".NET SDK is not installed. Please install .NET 8 SDK first."
    print_status "Download from: https://dotnet.microsoft.com/download"
    exit 1
fi

if ! command_exists docker; then
    print_warning "Docker is not installed. Docker features will be skipped."
    SKIP_DOCKER=true
else
    SKIP_DOCKER=false
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
print_success ".NET SDK version: $DOTNET_VERSION"

# Create project directory
PROJECT_NAME="TelematicsDataPlatform"
print_status "Creating project directory: $PROJECT_NAME"

if [ -d "$PROJECT_NAME" ]; then
    print_warning "Directory $PROJECT_NAME already exists. Do you want to remove it? (y/N)"
    read -r response
    if [[ "$response" =~ ^[Yy]$ ]]; then
        rm -rf "$PROJECT_NAME"
        print_status "Removed existing directory"
    else
        print_error "Aborting setup"
        exit 1
    fi
fi

mkdir "$PROJECT_NAME"
cd "$PROJECT_NAME"

# Create solution and projects
print_status "Creating solution and projects..."

dotnet new sln -n TelematicsDataPlatform

# Create projects
print_status "Creating Web API project..."
dotnet new webapi -n TelematicsApi

print_status "Creating Core library..."
dotnet new classlib -n TelematicsCore

print_status "Creating Data library..."
dotnet new classlib -n TelematicsData

print_status "Creating Batch Processor..."
dotnet new console -n TelematicsBatchProcessor

print_status "Creating Test project..."
dotnet new xunit -n TelematicsTests

# Add projects to solution
print_status "Adding projects to solution..."
dotnet sln add TelematicsApi/TelematicsApi.csproj
dotnet sln add TelematicsCore/TelematicsCore.csproj
dotnet sln add TelematicsData/TelematicsData.csproj
dotnet sln add TelematicsBatchProcessor/TelematicsBatchProcessor.csproj
dotnet sln add TelematicsTests/TelematicsTests.csproj

# Install packages for TelematicsCore
print_status "Installing packages for TelematicsCore..."
cd TelematicsCore
dotnet add package Microsoft.EntityFrameworkCore.Abstractions --version 8.0.0
dotnet add package NetTopologySuite --version 2.5.0
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions --version 8.0.0
dotnet add package Microsoft.Extensions.Logging.Abstractions --version 8.0.0
dotnet add package FluentValidation --version 11.9.0

# Install packages for TelematicsData
print_status "Installing packages for TelematicsData..."
cd ../TelematicsData
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package NetTopologySuite --version 2.5.0
dotnet add package NetTopologySuite.IO.GeoJSON --version 4.0.0
dotnet add reference ../TelematicsCore/TelematicsCore.csproj

# Install packages for TelematicsApi
print_status "Installing packages for TelematicsApi..."
cd ../TelematicsApi
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
dotnet add package Serilog.AspNetCore --version 8.0.0
dotnet add package Serilog.Sinks.Console --version 5.0.0
dotnet add package Serilog.Sinks.File --version 5.0.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.0
dotnet add package FluentValidation.AspNetCore --version 11.3.0
dotnet add reference ../TelematicsCore/TelematicsCore.csproj
dotnet add reference ../TelematicsData/TelematicsData.csproj

# Install packages for TelematicsBatchProcessor
print_status "Installing packages for TelematicsBatchProcessor..."
cd ../TelematicsBatchProcessor
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.Extensions.Hosting --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration --version 8.0.0
dotnet add package Microsoft.Extensions.Configuration.Json --version 8.0.0
dotnet add package Serilog.Extensions.Hosting --version 8.0.0
dotnet add package Serilog.Sinks.Console --version 5.0.0
dotnet add package Serilog.Sinks.File --version 5.0.0
dotnet add reference ../TelematicsCore/TelematicsCore.csproj
dotnet add reference ../TelematicsData/TelematicsData.csproj

# Install packages for TelematicsTests
print_status "Installing packages for TelematicsTests..."
cd ../TelematicsTests
dotnet add package Microsoft.EntityFrameworkCore.InMemory --version 8.0.0
dotnet add package Microsoft.AspNetCore.Mvc.Testing --version 8.0.0
dotnet add package FluentAssertions --version 6.12.0
dotnet add reference ../TelematicsCore/TelematicsCore.csproj
dotnet add reference ../TelematicsData/TelematicsData.csproj
dotnet add reference ../TelematicsApi/TelematicsApi.csproj

cd ..

# Install Entity Framework tools globally
print_status "Installing Entity Framework tools..."
dotnet tool install --global dotnet-ef --version 8.0.0 || print_warning "EF tools already installed or failed to install"

# Create directory structure
print_status "Creating directory structure..."

# TelematicsCore directories
mkdir -p TelematicsCore/Models
mkdir -p TelematicsCore/DTOs
mkdir -p TelematicsCore/Interfaces
mkdir -p TelematicsCore/Services

# TelematicsApi directories
mkdir -p TelematicsApi/Controllers

# TelematicsBatchProcessor directories
mkdir -p TelematicsBatchProcessor/Services

# Create configuration files
print_status "Creating configuration files..."

# Update TelematicsApi appsettings.json
cat > TelematicsApi/appsettings.json << 'EOF'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TelematicsDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/telematics-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  },
  "AllowedHosts": "*"
}
EOF

# Create TelematicsBatchProcessor appsettings.json
cat > TelematicsBatchProcessor/appsettings.json << 'EOF'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TelematicsDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/batch-processor-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
EOF

# Create Docker files if Docker is available
if [ "$SKIP_DOCKER" = false ]; then
    print_status "Creating Docker configuration..."
    
    # API Dockerfile
    cat > TelematicsApi/Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TelematicsApi/TelematicsApi.csproj", "TelematicsApi/"]
COPY ["TelematicsCore/TelematicsCore.csproj", "TelematicsCore/"]
COPY ["TelematicsData/TelematicsData.csproj", "TelematicsData/"]
RUN dotnet restore "TelematicsApi/TelematicsApi.csproj"
COPY . .
WORKDIR "/src/TelematicsApi"
RUN dotnet build "TelematicsApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelematicsApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelematicsApi.dll"]
EOF

    # Batch Processor Dockerfile
    cat > TelematicsBatchProcessor/Dockerfile << 'EOF'
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TelematicsBatchProcessor/TelematicsBatchProcessor.csproj", "TelematicsBatchProcessor/"]
COPY ["TelematicsCore/TelematicsCore.csproj", "TelematicsCore/"]
COPY ["TelematicsData/TelematicsData.csproj", "TelematicsData/"]
RUN dotnet restore "TelematicsBatchProcessor/TelematicsBatchProcessor.csproj"
COPY . .
WORKDIR "/src/TelematicsBatchProcessor"
RUN dotnet build "TelematicsBatchProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TelematicsBatchProcessor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TelematicsBatchProcessor.dll"]
EOF

    # Docker Compose
    cat > docker-compose.yml << 'EOF'
version: '3.8'

services:
  telematics-api:
    build:
      context: .
      dockerfile: TelematicsApi/Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=sql-server;Database=TelematicsDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true
    depends_on:
      - sql-server
    networks:
      - telematics-network

  batch-processor:
    build:
      context: .
      dockerfile: TelematicsBatchProcessor/Dockerfile
    environment:
      - ConnectionStrings__DefaultConnection=Server=sql-server;Database=TelematicsDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true
    depends_on:
      - sql-server
    networks:
      - telematics-network

  sql-server:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver_data:/var/opt/mssql
    networks:
      - telematics-network

volumes:
  sqlserver_data:

networks:
  telematics-network:
    driver: bridge
EOF
fi

# Create development scripts
print_status "Creating development scripts..."

# Run API script
cat > run-api.sh << 'EOF'
#!/bin/bash
echo "Starting Telematics API..."
cd TelematicsApi
dotnet run --urls "https://localhost:5001;http://localhost:5000"
EOF
chmod +x run-api.sh

# Run Batch Processor script
cat > run-batch.sh << 'EOF'
#!/bin/bash
echo "Starting Telematics Batch Processor..."
cd TelematicsBatchProcessor
dotnet run
EOF
chmod +x run-batch.sh

# Run Tests script
cat > run-tests.sh << 'EOF'
#!/bin/bash
echo "Running Telematics Tests..."
cd TelematicsTests
dotnet test --verbosity normal
EOF
chmod +x run-tests.sh

# Database setup script
cat > setup-database.sh << 'EOF'
#!/bin/bash
echo "Setting up database..."
cd TelematicsApi
echo "Creating migration..."
dotnet ef migrations add InitialCreate --project ../TelematicsData
echo "Updating database..."
dotnet ef database update --project ../TelematicsData
echo "Database setup complete!"
EOF
chmod +x setup-database.sh

# Docker setup script
if [ "$SKIP_DOCKER" = false ]; then
    cat > run-docker.sh << 'EOF'
#!/bin/bash
echo "Starting services with Docker Compose..."
docker-compose up --build
EOF
    chmod +x run-docker.sh
fi

# Create .gitignore
cat > .gitignore << 'EOF'
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/
x64/
x86/
[Aa][Rr][Mm]/
[Aa][Rr][Mm]64/
bld/
[Bb]in/
[Oo]bj/

# Visual Studio / Rider
.vs/
.idea/
*.user
*.userosscache
*.sln.docstates

# .NET
project.lock.json
project.fragment.lock.json
artifacts/

# NuGet
*.nupkg
**/[Pp]ackages/*
!**/[Pp]ackages/build/

# Entity Framework
**/Migrations/

# Logs
logs/
*.log

# Database
*.db
*.sqlite

# Environment variables
.env
.env.local
.env.development.local
.env.test.local
.env.production.local

# OS generated files
.DS_Store
.DS_Store?
._*
.Spotlight-V100
.Trashes
ehthumbs.db
Thumbs.db
EOF

# Create README
cat > README.md << 'EOF'
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
EOF

# Build the solution
print_status "Building the solution..."
dotnet build

if [ $? -eq 0 ]; then
    print_success "Build completed successfully!"
else
    print_error "Build failed. Please check the errors above."
    exit 1
fi

# Create Rider run configurations directory
print_status "Creating Rider run configurations..."
mkdir -p .idea/runConfigurations

# API Run Configuration
cat > .idea/runConfigurations/TelematicsApi.xml << 'EOF'
<component name="ProjectRunConfigurationManager">
  <configuration default="false" name="TelematicsApi" type="LaunchSettings" factoryName=".NET Launch Settings Profile">
    <option name="LAUNCH_PROFILE_PROJECT_FILE_PATH" value="$PROJECT_DIR$/TelematicsApi/TelematicsApi.csproj" />
    <option name="LAUNCH_PROFILE_TFM" value="net8.0" />
    <option name="LAUNCH_PROFILE_NAME" value="https" />
    <option name="USE_EXTERNAL_CONSOLE" value="0" />
    <option name="USE_MONO" value="0" />
    <option name="RUNTIME_ARGUMENTS" value="" />
    <option name="GENERATE_APPLICATIONHOST_CONFIG" value="1" />
    <option name="SHOW_IIS_EXPRESS_OUTPUT" value="0" />
    <option name="SEND_DEBUG_REQUEST" value="1" />
    <option name="ADDITIONAL_IIS_EXPRESS_ARGUMENTS" value="" />
    <method v="2">
      <option name="Build" />
    </method>
  </configuration>
</component>
EOF

# Batch Processor Run Configuration
cat > .idea/runConfigurations/TelematicsBatchProcessor.xml << 'EOF'
<component name="ProjectRunConfigurationManager">
  <configuration default="false" name="TelematicsBatchProcessor" type="DotNetProject" factoryName=".NET Project">
    <option name="EXE_PATH" value="$PROJECT_DIR$/TelematicsBatchProcessor/bin/Debug/net8.0/TelematicsBatchProcessor.dll" />
    <option name="PROGRAM_PARAMETERS" value="" />
    <option name="WORKING_DIRECTORY" value="$PROJECT_DIR$/TelematicsBatchProcessor" />
    <option name="PASS_PARENT_ENVS" value="1" />
    <option name="USE_EXTERNAL_CONSOLE" value="0" />
    <option name="USE_MONO" value="0" />
    <option name="RUNTIME_ARGUMENTS" value="" />
    <option name="PROJECT_PATH" value="$PROJECT_DIR$/TelematicsBatchProcessor/TelematicsBatchProcessor.csproj" />
    <option name="PROJECT_EXE_PATH_TRACKING" value="1" />
    <option name="PROJECT_ARGUMENTS_TRACKING" value="1" />
    <option name="PROJECT_WORKING_DIRECTORY_TRACKING" value="1" />
    <option name="PROJECT_KIND" value="DotNetCore" />
    <option name="PROJECT_TFM" value="net8.0" />
    <method v="2">
      <option name="Build" />
    </method>
  </configuration>
</component>
EOF

# Tests Run Configuration
cat > .idea/runConfigurations/Tests.xml << 'EOF'
<component name="ProjectRunConfigurationManager">
  <configuration default="false" name="Tests" type="DotNetTestRunConfigurationType" factoryName="XUnit">
    <option name="TEST_PROJECT_PATH" value="$PROJECT_DIR$/TelematicsTests/TelematicsTests.csproj" />
    <option name="TEST_KIND" value="PROJECT" />
    <method v="2">
      <option name="Build" />
    </method>
  </configuration>
</component>
EOF

print_success "Project setup completed successfully!"
print_status "Project location: $(pwd)"
echo ""
print_status "Next steps:"
echo "1. Open the project in JetBrains Rider: File -> Open -> Select the .sln file"
echo "2. Run database setup: ./setup-database.sh"
echo "3. Use the run configurations in Rider or run the shell scripts"
echo ""
print_status "Available run configurations in Rider:"
echo "- TelematicsApi (Web API)"
echo "- TelematicsBatchProcessor (Background service)"
echo "- Tests (Unit/Integration tests)"
echo ""
print_status "Manual commands:"
echo "- Start API: ./run-api.sh"
echo "- Start Batch Processor: ./run-batch.sh"
echo "- Run Tests: ./run-tests.sh"
if [ "$SKIP_DOCKER" = false ]; then
    echo "- Run with Docker: ./run-docker.sh"
fi
echo ""
print_success "Good luck with your interview! 🚀"