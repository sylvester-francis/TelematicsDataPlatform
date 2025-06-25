#!/bin/bash

# TelematicsHQ Full Stack Startup Script
echo "🚀 Starting TelematicsHQ Full Stack Container..."

# Function to check if service is running
check_service() {
    local service_name=$1
    local check_command=$2
    local max_attempts=30
    local attempt=1

    echo "⏳ Waiting for $service_name to start..."
    
    while [ $attempt -le $max_attempts ]; do
        if eval $check_command >/dev/null 2>&1; then
            echo "✅ $service_name is running"
            return 0
        fi
        
        echo "🔄 Attempt $attempt/$max_attempts - $service_name not ready yet..."
        sleep 2
        attempt=$((attempt + 1))
    done
    
    echo "❌ $service_name failed to start after $max_attempts attempts"
    return 1
}

# Configure API URL in frontend build if provided
if [ ! -z "$API_URL" ]; then
    echo "🔧 Configuring frontend API URL to: $API_URL"
    find /var/www/html -name "*.js" -type f -exec sed -i "s|http://localhost:5000/api|$API_URL|g" {} \;
else
    echo "🔧 Using default API configuration (relative URLs)"
fi

# Start nginx in background
echo "🌐 Starting nginx..."
nginx -t && nginx -g "daemon off;" &
NGINX_PID=$!

# Wait a moment for nginx to start
sleep 3

# Start .NET API in background
echo "🔗 Starting .NET API..."
cd /app/api
dotnet TelematicsApi.dll &
API_PID=$!

# Wait for services to be ready
check_service "nginx" "curl -f http://localhost/"
NGINX_STATUS=$?

check_service ".NET API" "curl -f http://localhost:5000/api/health"
API_STATUS=$?

# Check if both services started successfully
if [ $NGINX_STATUS -eq 0 ] && [ $API_STATUS -eq 0 ]; then
    echo "🎉 TelematicsHQ Full Stack is ready!"
    echo "📊 Frontend available at: http://localhost/"
    echo "🔗 Backend API available at: http://localhost:5000/api"
    echo "💚 Health check: http://localhost/health"
    echo "📈 API Health: http://localhost/api-health"
else
    echo "❌ Failed to start TelematicsHQ services"
    exit 1
fi

# Function to cleanup on exit
cleanup() {
    echo "🛑 Shutting down TelematicsHQ services..."
    kill $NGINX_PID 2>/dev/null
    kill $API_PID 2>/dev/null
    wait
    echo "✅ Shutdown complete"
}

# Set trap to cleanup on exit
trap cleanup SIGTERM SIGINT

# Wait for either process to exit
wait -n

# If we get here, one of the services exited
echo "⚠️ One of the services exited unexpectedly"
cleanup
exit 1