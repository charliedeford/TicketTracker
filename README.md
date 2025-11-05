# TicketTracker

TicketTracker is a full-stack web application built with ASP.NET Core and Blazor that allows organizations to manage and track support tickets efficiently.

## Features

- User Authentication and Authorization
- Role-based access control with support for multiple user groups
- Ticket Management
  - Create and update tickets
  - Assign tickets to users
  - Track ticket status
  - Paginated ticket listing
- Support for different user roles (Admin, Support, Regular Users)

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- A modern web browser

### Installation

1. Clone the repository:
```powershell
git clone https://github.com/charliedeford/TicketTracker.git
cd TicketTracker
```

2. Build the solution:
```powershell
dotnet build
```

3. Run the API and UI projects:
```powershell
# Run the API (in one terminal)
cd TicketTracker.Api
dotnet run

# Run the UI (in another terminal)
cd TicketTracker.Ui
dotnet run
```

### User Registration

To use the application:

1. Navigate to the registration page
2. Create an account by providing:
   - Username
   - Password
