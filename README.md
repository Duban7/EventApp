# Running the Application with Docker Compose

This guide will help you set up and run the application in an isolated environment using Docker Compose.

## Prerequisites

- [Docker](https://docs.docker.com/get-docker/) installed
- [Docker Compose](https://docs.docker.com/compose/install/) installed (usually included with Docker Desktop)

## Running the Application

1. Clone the repository (if not already done):
   ```bash
   git clone https://github.com/Duban7/EventApp
   cd EventApp
   ```

2. Start the application using Docker compose:
   ```bash
   docker-compose up -d
   ```

3. After startup, the application should be available at:
   + Web interface: http://localhost:4200
   + API (swagger): https://localhost:8080/swagger

4. Stopping tha application:
   ```bash
   docker-compose down
   ```
# Admin

The application has a pre-installed administrator. Use these details to access the administrator:
   + Login: admin@mail.com
   + Password: admin1337
