# Change: Refactor backend to Clean Architecture with EF Core + PostgreSQL

## Why
The backend currently consists of a single ASP.NET Core minimal API project with development reverse-proxy wiring, but it does not yet establish clear boundaries for domain rules, use cases, and persistence. We want a Clean Architecture structure so business rules stay testable and persistence/framework concerns remain replaceable.

We also need to introduce a real persistence layer using EF Core + PostgreSQL so the application can store tenant-scoped data and evolve toward multi-tenant SaaS readiness.

## What Changes
- Refactor the backend into explicit Clean Architecture layers (Domain, Application, Infrastructure, API/Presentation).
- Introduce EF Core with the Npgsql provider and a PostgreSQL database for persistence.
- Define a migration strategy that keeps the developer workflow simple (retain `task dev` as the happy path).
- Establish multi-tenancy enforcement conventions at the persistence boundary (tenant-scoped queries must be filtered by the current `TenantId`).
- Add initial testing and validation scaffolding for the new architecture (unit tests for domain/application; integration tests against Postgres).

## Impact
- Affected specs:
  - `structure-backend`
  - `persist-data`
- Affected code:
  - `backend/MiLyst.Api/*` (will become presentation layer and composition root)
  - New backend projects under `backend/` (Domain/Application/Infrastructure)
  - `Taskfile.yml` (dev workflow updates for Postgres)
  - `MiLyst.slnx` (solution structure)
  - Backend configuration files (`appsettings*.json`, environment variables)
