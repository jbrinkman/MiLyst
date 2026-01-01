## ADDED Requirements

### Requirement: Layered Backend Structure
The backend SHALL be organized into separate layers (Domain, Application, Infrastructure, API) that follow the Clean Architecture dependency rule.

#### Scenario: Layered projects compile with correct dependency direction
- **WHEN** the backend solution is built
- **THEN** the Domain layer compiles without referencing ASP.NET Core or EF Core
- **AND** dependencies only point inward (API -> Application, Infrastructure; Infrastructure -> Application, Domain; Application -> Domain)

### Requirement: Composition Root
The API layer SHALL act as the composition root that wires dependencies and configures cross-cutting concerns.

#### Scenario: Dependencies are resolved through DI at runtime
- **WHEN** the API host starts
- **THEN** all Application and Infrastructure services required by endpoints can be resolved via dependency injection

### Requirement: Use Case Isolation
HTTP endpoints SHALL invoke Application-layer use cases rather than embedding business logic or persistence logic in the API layer.

#### Scenario: Endpoint execution delegates to an application use case
- **WHEN** an HTTP request is handled by an endpoint
- **THEN** the endpoint maps request data to an Application use case
- **AND** the endpoint returns an HTTP response mapped from the use case result
