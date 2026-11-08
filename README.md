# RaceDay – Part 2: RESTful API

RaceDay is a web-based event management system for the South African road running, walking and cycling community. This repository holds the Part 2 RESTful API, built with ASP.NET Core Web API (C#) and SQL Server. Organisers create and manage events, categories and results. Participants browse events, enrol in a category and view their own results.

Part 1 (planning documents, ERD, SQL script) is in the /docs folder.

The two roles
Role	What they can do
Organiser	Create, update and delete their own events. Create categories and attach them to events (entry fee, maximum participants). View all enrolments for their events. Capture and update results (finish time, position, status).
Participant	Register and log in. Browse events and categories. Enrol in an event by choosing a category. View and cancel their own enrolments. View their own results.

Both roles can view and update their own profile.

How roles are managed: anyone may register as a Participant. Registering as an Organiser requires a secret organiser code (OrganiserRegistrationCode in appsettings.json), so a normal user cannot make themselves an Organiser.

Technology
ASP.NET Core Web API (.NET 8)
SQL Server (LocalDB) using System.Data.SqlClient with parameterised queries
BCrypt (BCrypt.Net-Next) for password hashing
Server-side session (user id and role stored after login)
Swagger (Swashbuckle) for live endpoint documentation
GitHub Actions for CI
Setup and run instructions
Prerequisites
Visual Studio 2022 with the ASP.NET workload (or the .NET 8 SDK)
SQL Server LocalDB and SQL Server Management Studio (SSMS)
1. Create the database
Open SSMS and connect to (localdb)\api_both.
Open docs/raceday_schema.sql and execute it. This creates the raceday database, all tables and the seed data for roles and categories.
2. Check the configuration
The connection string used by the data classes is: Data source=(localdb)\api_both;database=raceday;
appsettings.json must contain the organiser code:
json
  "OrganiserRegistrationCode": "RACEDAY-ORG-2026"
3. Install packages
dotnet add package System.Data.SqlClient
dotnet add package BCrypt.Net-Next
dotnet add package Swashbuckle.AspNetCore
4. Run the API
dotnet run --project RaceDay

Open https://localhost:7034/swagger (the port may differ on your machine).

5. Seed users

Users cannot be seeded with plain SQL because passwords must be BCrypt hashes. Register them through Swagger (POST /api/auth/register):

Role	Example values
Organiser	role_name Organiser, organiser_code RACEDAY-ORG-2026
Participant	role_name Participant

Then log in with POST /api/auth/login. Swagger keeps the session cookie, so protected endpoints work in the same browser tab.

Endpoints

Inputs are sent as query/path parameters. Protected endpoints return 401 when not logged in and 403 when the role or ownership is wrong.

Authentication
Method	Route	Role
POST	/api/auth/register	Public
POST	/api/auth/login	Public
POST	/api/auth/logout	Any
User profile
Method	Route	Role
GET	/api/users/profile	Participant / Organiser
PUT	/api/users/profile	Participant / Organiser
Events
Method	Route	Role
GET	/api/events	Public
GET	/api/events/{id}	Public
GET	/api/events/my	Organiser
POST	/api/events	Organiser
PUT	/api/events/{id}	Organiser (owner)
DELETE	/api/events/{id}	Organiser (owner)
Categories
Method	Route	Role
GET	/api/categories	Public
GET	/api/categories/{id}	Public
POST	/api/categories	Organiser
PUT	/api/categories/{id}	Organiser
DELETE	/api/categories/{id}	Organiser
Event categories
Method	Route	Role
GET	/api/events/{event_id}/categories	Public
POST	/api/events/{event_id}/categories	Organiser (owner)
PUT	/api/event-categories/{id}	Organiser (owner)
DELETE	/api/event-categories/{id}	Organiser (owner)
Enrolments
Method	Route	Role
POST	/api/events/{event_id}/enrolments	Participant
GET	/api/enrolments/my	Participant
GET	/api/events/{event_id}/enrolments	Organiser (owner)
GET	/api/enrolments/{id}	Participant (own) / Organiser (owner)
DELETE	/api/enrolments/{id}	Participant (own)
Results
Method	Route	Role
POST	/api/enrolments/{enrolment_id}/results	Organiser (owner)
PUT	/api/results/{id}	Organiser (owner)
GET	/api/results/my	Participant
GET	/api/events/{event_id}/results	Organiser (owner)
GET	/api/results/{id}	Participant (own) / Organiser (owner)

Finish times use the format hh:mm:ss. Event types are Run, Walk or Cycle. Result statuses are Finished, DNF, DNS or Disqualified.

Authentication and security
Passwords are hashed with BCrypt. Plain-text passwords are never stored.
On login the user id and role are stored in a server-side session.
Every non-public endpoint checks the session (401) and the role (403).
Organisers can only change or view data for events they own.
All SQL uses parameters to prevent SQL injection.
Deviations from the Part 1 plan
Change	Reason
register accepts username, phone_number, role_name and organiser_code. A wrong organiser code returns 403.	Role selection must be controlled (Part 2 requirement).
PUT /api/users/profile also accepts username and phone_number.	Matches the users table.
POST /api/events takes the organiser from the session, not the request body.	Prevents creating events as another organiser.
Added POST /api/auth/logout.	Needed to end a session (used in Part 3).
Added GET /api/events/{event_id}/categories.	Participants need the category ids to enrol.
GET /api/enrolments/my returns 200 OK.	The Part 1 table listed the POST response there by mistake.
Inputs are query parameters instead of JSON bodies.	Follows the lecturer's sample structure.
Data access uses ADO.NET (SqlClient) instead of Entity Framework Core.	Follows the lecturer's sample structure. (Confirm with your lecturer and edit this line if needed.)
Unit tests

The RaceDay.Tests project covers registration, login, event management, role rejection and enrolments (success and failure cases).

dotnet test

(Update this section once the tests are written and passing locally and in CI.)

CI/CD

The GitHub Actions workflow is in .github/workflows/ci.yml. It builds the project and runs the unit tests on every push.

Green build screenshot:

![CI green build](docs/ci-green-build.png)

(Replace with your screenshot once the build is green.)

Video presentation

YouTube (unlisted): [https://youtu.be/imxpIHLfMf4]

AI usage disclosure

AI assistance (Claude) was used for planning the endpoint structure, generating a first draft of parts of the code and debugging errors. All code was reviewed, tested and is understood by the author, who can explain it in the video. (Edit this note so it accurately reflects your own use.)

Author

Student name: [GEORGE KINSMAN MATSHEBELE] Student number: [ST10477632] Module: PROG6212 – Programming 2B
