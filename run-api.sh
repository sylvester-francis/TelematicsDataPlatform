#!/bin/bash
echo "Starting Telematics API..."
cd TelematicsApi
dotnet run --urls "https://localhost:5001;http://localhost:5000"
