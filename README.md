# MiLyst

A modern task management application with kanban/scrum board interface for tracking and organizing work.

## Features

- Intuitive kanban board interface
- Task creation and management
- Progress tracking
- Clean, responsive design

## Getting Started

### Prerequisites

- .NET SDK 10
- Node.js + npm
- Docker (required for local PostgreSQL and integration tests)

### Development

Configure the database connection string (a local development default is provided in `appsettings*.json`, but you can override it):

The default connection string uses development-only credentials and MUST be overridden for production.

Option A: Use user secrets (recommended for local development):

```sh
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=milyst_dev;Username=milyst;Password=milyst" --project backend/MiLyst.Api
```

Option B: Use an environment variable (recommended for CI/production):

```sh
export ConnectionStrings__DefaultConnection='Host=localhost;Port=5432;Database=milyst_dev;Username=milyst;Password=milyst'
```

Install dependencies:

```sh
task deps
```

Start the backend (starts local Postgres automatically and proxies Vite in Development):

```sh
task dev
```

### Database

Start/stop Postgres:

```sh
task postgres:start
task postgres:status
task postgres:stop
```

Apply EF Core migrations:

```sh
task db:migrate
```

### Tests

Unit tests:

```sh
task test
```

Integration tests (requires Docker):

```sh
task test:integration
```

## License

This project is licensed under the BSD 3-Clause License - see the [LICENSE](LICENSE) file for details.
