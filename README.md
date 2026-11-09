# RaceDayWeb

## Project Description

RaceDayWeb is an ASP.NET Core MVC application developed using .NET 8.

The application consumes the RaceDay REST API to provide functionality
for event organisers and participants.

## Technologies Used

- C#
- ASP.NET Core MVC
- .NET 8
- SQL Server
- REST API
- HTML
- CSS
- Bootstrap 5
- JavaScript
- Docker
- GitHub Actions

## Main Features

### Organiser

- Register and log in as an organiser.
- View the organiser dashboard.
- Create, edit and delete events.
- Manage event categories.
- View participant enrolments.
- Capture participant race results.

### Participant

- Register and log in as a participant.
- Browse upcoming events.
- View event information.
- Enrol in events.
- View personal enrolments.
- View race results.
- Update personal profile information.

## Project Structure

RaceDayWeb
- Controllers
- Models
- Models/ViewModels
- Services
- Views
- wwwroot
- Properties
- Program.cs
- appsettings.json
- Dockerfile
- README.md

## Requirements

Install the following before running the application:

- Visual Studio 2022
- .NET 8 SDK
- SQL Server or SQL Server LocalDB
- RaceDay REST API

## How to Run the Application

1. Open the RaceDay API project in Visual Studio.
2. Make sure the API database is configured correctly.
3. Run the API project.
4. Check the API address displayed when it starts.
5. Open the RaceDayWeb project.
6. Open appsettings.json.
7. Update ApiSettings:BaseUrl to match the running API address.
8. Set RaceDayWeb as the startup project.
9. Build the solution.
10. Run the MVC application.

## Configuration

The API address is configured in appsettings.json:

    {
      "ApiSettings": {
        "BaseUrl": "https://localhost:7100/"
      }
    }

Change the URL if your RaceDay API uses a different address.

## Docker

Build the Docker image:

    docker build -t racedayweb .

Run the container:

    docker run -p 8080:8080 racedayweb

Open:

    http://localhost:8080

The RaceDay API must also be running and reachable by the MVC application.

## Demonstration Video

YouTube demonstration:

Add your unlisted YouTube demonstration URL here.

## Developer

RaceDay Part 3 - MVC Application
