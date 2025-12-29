## ADDED Requirements

### Requirement: PostgreSQL Persistence
The system SHALL use PostgreSQL as the primary persistence store for application data.

#### Scenario: Application can connect to PostgreSQL
- **WHEN** the application is started with a valid PostgreSQL connection string
- **THEN** the application can open database connections and execute EF Core queries successfully

### Requirement: EF Core Data Access
The system SHALL use EF Core (with the Npgsql provider) for database access and mapping.

#### Scenario: Data access is implemented through EF Core in Infrastructure
- **WHEN** an Application use case performs a data read or write
- **THEN** the data access is executed through Infrastructure implementations backed by EF Core

### Requirement: Migrations Support
The system SHALL provide a supported workflow to create and apply EF Core migrations for PostgreSQL.

#### Scenario: Migrations can be applied to a developer database
- **WHEN** a developer runs the documented migration workflow
- **THEN** the database schema is created/updated to match the current EF Core model

### Requirement: Tenant-Scoped Data Isolation
All tenant-scoped data SHALL be persisted with a `TenantId` and tenant-scoped queries SHALL be filtered by the current tenant context.

#### Scenario: Cross-tenant data is not returned
- **WHEN** a request is executed under tenant A
- **THEN** reads for tenant-scoped entities do not return data belonging to tenant B

### Requirement: Local Development Database Workflow
The system SHALL provide a straightforward workflow to run PostgreSQL locally for development.

#### Scenario: Developer can start PostgreSQL for local development
- **WHEN** a developer follows the documented dev workflow
- **THEN** a PostgreSQL instance is available for the backend to connect to
