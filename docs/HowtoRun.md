# How to Run the Assecor API

This document describes various ways to run and test the Assecor Person API.

## Prerequisites

- .NET 10 SDK
- Docker (optional, for containerized deployment)
- Visual Studio 2026 or later

## Running the Application

### Option 1: Using .NET CLI

Navigate to the project directory and run:

```bash
cd src/Assecor.Api.Person
dotnet run
```

To run it with Swagger UI enabled (Development mode), use:
```bash
cd src/Assecor.Api.Person
dotnet run --launch-profile https
```


Or to run with a specific launch profile:

```bash
# Run with HTTPS (default Development mode)
dotnet run --launch-profile https

# Run with HTTP only
dotnet run --launch-profile http
```


### Option 2: Using Docker

#### Build and Run the Docker Image

From the repository root directory:

```bash
# Build the Docker image
docker build -t assecor-api -f src/Assecor.Api.Person/Dockerfile .

# Run the container (Production mode - no Swagger)
docker run -d -p 8080:8080 -p 8081:8081 --name assecor-api-container assecor-api

# OR: Run with Development environment (enables Swagger)
docker run -d -p 8080:8080 -p 8081:8081 -e ASPNETCORE_ENVIRONMENT=Development --name assecor-api-container assecor-api
```

The application will be available at:
- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081
- Swagger (Development mode only): http://localhost:8080/swagger

#### View Container Logs

```bash
# View real-time logs
docker logs -f assecor-api-container

# View last 100 lines
docker logs --tail 100 assecor-api-container
```

#### Stop and Remove the Container

```bash
docker stop assecor-api-container
docker rm assecor-api-container
```

#### Useful Docker Commands

```bash
# List running containers
docker ps

# List all containers (including stopped)
docker ps -a

# Remove the image
docker rmi assecor-api

# Execute commands inside the running container
docker exec -it assecor-api-container /bin/bash
```

## Application URLs

### Development Mode (with Swagger)
- HTTPS: https://localhost:7179
- HTTP: http://localhost:5277
- Swagger UI: https://localhost:7179/swagger or http://localhost:5277/swagger
- OpenAPI spec: https://localhost:7179/openapi/v1.json

### Docker Container Mode
- HTTP: http://localhost:8080
- HTTPS: https://localhost:8081
- Swagger: Available at /swagger (only when running with `-e ASPNETCORE_ENVIRONMENT=Development`)

## Testing the API

### Option 1: Using Swagger UI

When running in **Development** mode, Swagger UI is automatically available:

1. Navigate to https://localhost:7179/swagger
2. Explore the available endpoints
3. Test requests directly from the Swagger UI

**Note**: Swagger is only available in Development mode. To enable it in other environments, modify the environment variable `ASPNETCORE_ENVIRONMENT=Development`.

### Option 2: Using HTTP Files (Visual Studio/JetBrains Rider)

The project includes an HTTP request file for testing: `src/Assecor.Api.Person/Assecor.Api.Person.http`

Available test requests:
- **Get all persons**: `GET /persons`
- **Get person by ID**: `GET /persons/{id}`
- **Get persons by color**: `GET /persons/color/{color}`
- **Create a new person**: `POST /persons`

To use in Visual Studio:
1. Open the `Assecor.Api.Person.http` file
2. Click the "Send Request" link above each request
3. View the response in the output window

### Option 3: Using curl

```bash
# Get all persons
curl -X GET "http://localhost:5277/persons" -H "accept: application/json"

# Get person by ID
curl -X GET "http://localhost:5277/persons/1" -H "accept: application/json"

# Get persons by color
curl -X GET "http://localhost:5277/persons/color/Blau" -H "accept: application/json"

# Create a new person
curl -X POST "http://localhost:5277/persons" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Max",
    "lastname": "Mustermann",
    "zipcode": "12345",
    "city": "Berlin",
    "color": "Blau"
  }'
```

### Option 4: Using Postman or Similar Tools

Import the API endpoints manually or use the OpenAPI spec:
- OpenAPI Spec URL: http://localhost:5277/openapi/v1.json (when running in Development mode)

## Configuration

### Data Source Configuration

The application supports two data sources, configured in `appsettings.json`:

#### CSV Mode (Default)
```json
"SqlOptions": {
  "UseSql": false
},
"CsvOptions": {
  "FilePath": "../Assecor.Api.Infrastructure/Csv/sample-input.csv",
  "Delimiter": ","
}
```

#### SQLite Mode
```json
"SqlOptions": {
  "UseSql": true
},
"ConnectionStrings": {
  "PersonDb": "Data Source=../Assecor.Api.Infrastructure/Sql/persons.db"
}
```

### Log Files

Logs are written to multiple locations as configured in `appsettings.json`:

- **File logs**: `log/log.log` (relative to the application directory)
  - Rolling interval: Daily
  - File size limit: 500 MB
  - Old logs are rolled over when size limit is reached
- **Console logs**: Output to the console with color-coded levels
- **Debug logs**: Output to the debug window (when debugging)

**Log file location**:
- When running via `dotnet run`: `src/Assecor.Api.Person/log/log.log`
- When running from Visual Studio: `src/Assecor.Api.Person/bin/Debug/net10.0/log/log.log`
- When running in Docker: Inside the container at `/app/log/log.log`

To access Docker container logs:
```bash
# View application logs from console output
docker logs assecor-api-container

# Access the log file inside the container
docker exec -it assecor-api-container cat /app/log/log.log
```

## Troubleshooting

### Docker: "No connection could be made"
**For Docker**:
```bash
docker run -d -p 9080:8080 -p 9081:8081 --name assecor-api-container assecor-api
# Access at http://localhost:9080
```

### Docker Container Cannot Access CSV/Database Files
The Dockerfile copies the source files during build. If you need to mount volumes for development:
```bash
docker run -d -p 8080:8080 -p 8081:8081 \
  -v ${PWD}/src/Assecor.Api.Infrastructure:/app/Infrastructure \
  -e ASPNETCORE_ENVIRONMENT=Development \
  --name assecor-api-container assecor-api
```

**Note**: On Windows PowerShell, use `${PWD}`. On Command Prompt, use `%cd%`.

### SSL Certificate Issues in Docker
If you encounter SSL certificate warnings when accessing https://localhost:8081 in Docker, this is expected. The container uses development certificates. Use HTTP (port 8080) instead, or accept the certificate warning in your browser/testing tool.



