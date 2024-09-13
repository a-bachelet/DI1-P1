# Why So Serious? (WSS)

<!-- COVERAGE_RESULTS -->

This repository contains two applications developed using .NET 8:
1. **WebAPI**: A backend API for handling data and WebSocket connections.
2. **CLI**: A command-line application that communicates with the WebAPI using SignalR and displays UI using `Terminal.Gui`.

Additionally, the project includes a PostgreSQL database and PgAdmin setup using Docker for managing the database. 

## Table of Contents
- [Prerequisites](#prerequisites)
- [Cloning the Repository](#cloning-the-repository)
- [Setting up Docker (PostgreSQL and PgAdmin)](#setting-up-docker-postgresql-and-pgadmin)
- [Running the WebAPI](#running-the-webapi)
- [Running the CLI](#running-the-cli)
- [Publishing the Applications](#publishing-the-applications)
- [Configuration](#configuration)
- [Troubleshooting](#troubleshooting)

## Prerequisites
Before you begin, ensure you have the following installed on your machine:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Git](https://git-scm.com/)

## Cloning the Repository
To clone the repository, run the following command:

```bash
git clone https://github.com/a-bachelet/DI1-P1
cd DI1-P1
```

## Setting up Docker (PostgreSQL and PgAdmin)

The project includes a Docker Compose file for setting up PostgreSQL and PgAdmin. To start the database services, navigate to the project root and run:

```bash
docker-compose up -d
```

This will:

- Spin up a PostgreSQL container for the database.
- Spin up a PgAdmin container for database management.

You can access PgAdmin in your browser at http://localhost:8080 with the following credentials:

- Email: wss@wss.com
- Password: WSS

## Running the WebAPI

Navigate to the Server project directory:

```bash
cd Server
```

Update the appsettings.json file if necessary (default configuration connects to PostgreSQL via localhost).

Run the WebAPI:

```bash
dotnet run --launch-profile https
```

The WebAPI will start running on https://localhost:7032 (as per your configuration).

## Running the CLI

Navigate to the Client project directory:

```bash
cd Client
```

Ensure the appsettings.json file contains the correct API and WebSocket server configuration. The default points to the WebAPI running locally.

Run the CLI application:

```bash
dotnet run
```

This will initialize the TUI (Terminal User Interface) which connects to the WebAPI and WebSocket server.

## Publishing the Applications

### Publishing the WebAPI

To publish the WebAPI to a folder or server, run the following command in the Server project directory:

```bash
dotnet publish -c Release -o ./publish
```

This will generate the compiled API in the ./publish folder, ready for deployment.

### Publishing the CLI

Similarly, to publish the CLI application, run the following command in the Client project directory:

```bash
dotnet publish -c Release -o ./publish
```

This will generate the compiled CLI in the ./publish folder.

## Configuration

### WebAPI Configuration

The WebAPI uses appsettings.json for configuration. The default configuration can be found in Server/appsettings.json. Key settings include:

Database Configuration:

```json
"Database": {
  "Host": "127.0.0.1",
  "Port": "5432",
  "Name": "wss_dev",
  "User": "wss",
  "Pass": "WSS"
}
```

### CLI Configuration

The CLI application uses Client/appsettings.json to configure the API and WebSocket server connections:

- API Configuration:

```json
"WebApiServer": {
  "Scheme": "https",
  "Domain": "localhost",
  "Port": "7032"
}
```

- WebSocket Server Configuration:

```json
"WebSocketServer": {
  "Scheme": "wss",
  "Domain": "localhost",
  "Port": "7032"
}
```

## Troubleshooting
- PostgreSQL Connection Issues: Ensure that the PostgreSQL container is running and accessible via port 5432.
- SSL Errors: If running locally without SSL, modify the scheme in appsettings.json to http instead of https for the WebAPI.
