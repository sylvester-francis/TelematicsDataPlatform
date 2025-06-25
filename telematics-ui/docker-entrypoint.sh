#!/bin/sh

# Replace API URL if environment variable is provided
if [ ! -z "$API_URL" ]; then
    echo "Configuring API URL to: $API_URL"
    find /usr/share/nginx/html -name "*.js" -exec sed -i "s|http://localhost:5000/api|$API_URL|g" {} \;
fi

# Start nginx
exec "$@"