TicketTracker.Api - local development

This is a minimal ASP.NET Core Web API that provides Users and Groups tables and simple username/password authentication that returns JWT tokens.

Prerequisites (Windows PowerShell):
- .NET SDK (recommended latest stable)
- (Optional) dotnet-ef tool: Install with `dotnet tool install --global dotnet-ef`

Quick start (PowerShell):
1. Restore packages:
   dotnet restore TicketTracker.Api\

2. Create EF migrations and update database (from workspace root):
   # install tool if needed: dotnet tool install --global dotnet-ef
   dotnet ef migrations add Initial -p TicketTracker.Api -s TicketTracker.Api; dotnet ef database update -p TicketTracker.Api -s TicketTracker.Api

3. Run API:
   dotnet run --project TicketTracker.Api

Endpoints:
- POST /api/auth/register  { username, password, groupIds? }
- POST /api/auth/login     { username, password }

Notes:
- Replace the JWT key in `appsettings.json` with a secure secret for production.
- This uses SQLite for local development (tickettracker.db).
