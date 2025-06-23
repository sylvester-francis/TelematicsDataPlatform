#!/bin/bash
echo "Setting up database..."
cd TelematicsApi
echo "Creating migration..."
dotnet ef migrations add InitialCreate --project ../TelematicsData
echo "Updating database..."
dotnet ef database update --project ../TelematicsData
echo "Database setup complete!"
