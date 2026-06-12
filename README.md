# Smart Parking System

Smart Parking System is a full-stack parking reservation application developed with ASP.NET Core Web API, React, SQL Server, JWT Authentication, SignalR, and Docker.

## Technologies Used

* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* React
* Vite
* JWT Authentication
* SignalR
* Docker
* Docker Compose

## Features

* User login with JWT authentication
* Role-based authorization for Admin and Customer users
* Parking spot listing
* Reservation creation
* Reservation completion
* Admin dashboard
* Real-time parking spot updates with SignalR
* Dockerized backend and frontend

## Running the Project with Docker

First, create a `.env` file in the project root:

```env
DB_USER=your_db_user
DB_PASSWORD=your_db_password
```

Then run:

```bash
docker compose up --build
```

Frontend:

```text
http://localhost:5173
```

Backend Swagger:

```text
http://localhost:5005/swagger
```

## Notes

SQL Server is currently expected to run on the host machine. The API connects to SQL Server using `host.docker.internal`.
