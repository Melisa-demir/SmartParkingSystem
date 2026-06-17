# Smart Parking System

Smart Parking System is a full-stack parking management project built with .NET, React, SQL Server, Docker, and a microservice architecture.

The project allows users to view parking spots, create reservations, complete reservations, and process payments through separate backend services. All microservices are connected through an API Gateway.

## Project Purpose

The main purpose of this project is to demonstrate:

* Microservice architecture
* API Gateway usage
* Service-to-service communication
* Docker Compose orchestration
* SQL Server integration
* Full-stack development with React and .NET
* Clean separation of responsibilities between services

## Architecture

The project is divided into multiple services:

```text
Frontend React App
        |
        v
API Gateway
        |
        |---- ParkingService
        |---- ReservationsService
        |---- PaymentService
        |
        v
SQL Server
```

## Microservices

### ParkingService

Responsible for managing parking lots and parking spots.

Main responsibilities:

* List parking spots
* Create parking spots
* Update parking spot status
* Mark a parking spot as occupied
* Release a parking spot

### ReservationsService

Responsible for reservation operations.

Main responsibilities:

* Create reservations
* List reservations
* Complete reservations
* Communicate with ParkingService to check and update parking spot status

### PaymentService

Responsible for payment operations.

Main responsibilities:

* Create payment records
* List payments
* Get reservation price from ReservationsService
* Prevent duplicate paid payments for the same reservation

### ApiGateway

Responsible for routing client requests to the correct microservice.

The API Gateway is implemented using Ocelot.

## Technologies Used

* .NET 8 Web API
* Entity Framework Core
* SQL Server
* React
* Vite
* Docker
* Docker Compose
* Ocelot API Gateway
* Swagger

## API Gateway Routes

### Parking Spots

```text
GET    /gateway/parking-spots
POST   /gateway/parking-spots
GET    /gateway/parking-spots/{id}
PUT    /gateway/parking-spots/{id}
PUT    /gateway/parking-spots/{id}/occupy
PUT    /gateway/parking-spots/{id}/release
```

### Parking Lots

```text
GET    /gateway/parking-lots
POST   /gateway/parking-lots
GET    /gateway/parking-lots/{id}
PUT    /gateway/parking-lots/{id}
DELETE /gateway/parking-lots/{id}
```

### Reservations

```text
GET    /gateway/reservations
POST   /gateway/reservations
GET    /gateway/reservations/{id}
PUT    /gateway/reservations/{id}/complete
DELETE /gateway/reservations/{id}
```

### Payments

```text
GET    /gateway/payments
POST   /gateway/payments
GET    /gateway/payments/{id}
```

## Docker Ports

| Service             | URL                   |
| ------------------- | --------------------- |
| API Gateway         | http://localhost:7100 |
| ParkingService      | http://localhost:8018 |
| ReservationsService | http://localhost:8259 |
| PaymentService      | http://localhost:8062 |
| SQL Server          | localhost,1434        |
| React Frontend      | http://localhost:5173 |

## Running the Project with Docker

Run the following command from the project root folder:

```bash
docker compose -f docker-compose.microservices.yml up --build
```

This command starts:

* SQL Server
* ParkingService
* ReservationsService
* PaymentService
* API Gateway

After the containers are running, open:

```text
http://localhost:7100/gateway/parking-spots
```

## Running the Frontend

Go to the frontend folder:

```bash
cd smart-parking-client
```

Install packages:

```bash
npm install
```

Run the React application:

```bash
npm run dev
```

Open the frontend in the browser:

```text
http://localhost:5173
```

## Example Flow

The main flow of the system is:

```text
1. User opens the React frontend.
2. Frontend sends requests to the API Gateway.
3. API Gateway routes requests to the correct microservice.
4. User creates a reservation.
5. ReservationsService checks ParkingService.
6. Parking spot is marked as occupied.
7. User creates a payment.
8. PaymentService gets reservation information.
9. User completes the reservation.
10. Parking spot is released again.
```

## Database

Each microservice uses its own database context:

* ParkingServiceDb
* ReservationServiceDb
* PaymentServiceDb

Entity Framework Core migrations are applied automatically when the services start in Docker.

## Current Status

Completed:

* Microservice structure
* API Gateway routing
* Docker Compose setup
* SQL Server container
* Parking spot management
* Reservation flow
* Payment flow
* React frontend integration with API Gateway

Planned improvements:

* Separate AuthService
* Better UI design
* Role-based authorization
* Centralized logging
* Message broker integration
* Unit and integration tests

## Notes

The current frontend uses a temporary development login flow to demonstrate the microservice-based parking, reservation, and payment operations. Authentication can be moved into a separate AuthService as a future improvement.

## Author

Melisa Demir
