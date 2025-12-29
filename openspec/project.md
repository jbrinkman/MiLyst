# Project Context

## Purpose
MiLyst is a modern task management application with a kanban/scrum board interface for tracking and organizing work.

## Tech Stack
- Backend:
  - .NET (ASP.NET Core), minimal APIs
  - EF Core + PostgreSQL
  - Auth: ASP.NET Core Identity
  - Validation: FluentValidation
  - Development reverse proxy: YARP (proxies to Vite)
- Frontend:
  - Vue 3 + TypeScript + Vite
  - Routing: Vue Router
  - UI/styling: UnoCSS (Wind4 preset) + shadcn-vue style config (reka-ui) + lucide icons
- Tooling:
  - Taskfile (`Taskfile.yml`) for common workflows
  - npm (root + `frontend/`) and dotnet CLI
  - Husky + commitlint (Conventional Commits) + DCO sign-off

## Project Conventions

### Code Style
- Prefer small, composable modules with explicit names over premature abstraction.
- Keep API contracts and domain rules explicit (avoid leaking persistence concerns into domain logic).

- Frontend:
  - Vue SFCs with `<script setup lang="ts">`.
  - Use the `@/` alias (maps to `frontend/src`).
  - Prefer shared UI components under `@/components/ui/*`.

- Backend:
  - Minimal API routing grouped under `/api`.
  - Prefer dependency injection and clear boundaries between layers.
  - Use FluentValidation for request validation (prefer fail-fast validation close to request boundaries).

### Architecture Patterns
- Backend uses a Clean Architecture approach with a layered structure (not fully implemented yet):
  - Domain: core business rules and entities (no framework/persistence dependencies)
  - Application: use cases, orchestration, validation, DTOs
  - Infrastructure: EF Core/Postgres, Identity implementation details, external services
  - API/Presentation: HTTP endpoints, request/response mapping

- Hosting model:
  - Development:
    - Backend runs on `http://127.0.0.1:5145` (see `Taskfile.yml`).
    - Vite dev server runs on `http://localhost:5173` and is proxied by the backend in development.
  - Production:
    - Backend serves the built SPA static assets and uses SPA fallback to `index.html`.

- Multi-tenancy:
  - The app is multi-tenant and multi-user using ASP.NET Core Identity.
  - Tenant isolation strategy: single database with `TenantId` on tenant-scoped data.
  - Tenant identification strategy: resolve current tenant from an HTTP request header (keep URLs clean).
  - Identity considerations:
    - ASP.NET Core Identity is not tenant-aware by default; tenancy rules must be enforced in application/domain logic.
    - A single user can be associated with multiple tenants.
    - **Convention**: all tenant-scoped queries MUST be filtered by the current `TenantId`.
    - **Convention**: do not trust client-provided tenant identifiers without authorization checks (e.g., ensure the authenticated user is a member of the tenant).

- Frontend routing:
  - Use Vue Router and ensure routes work with the backend’s SPA fallback behavior.
  - API calls should be made to `/api/*` endpoints.

### Testing Strategy
- Backend:
  - Unit tests: xUnit
  - Integration tests: xUnit + ASP.NET Core `WebApplicationFactory` test host
  - **Recommended**: use ephemeral Postgres for integration tests (e.g., container-based DB) once infrastructure is in place.

- Frontend:
  - Unit/component tests: **recommended** Vitest + Vue Test Utils (and Testing Library if desired)
  - E2E tests: Playwright

### Git Workflow
- Commit requirements:
  - DCO sign-off is required for every commit (use `git commit -s`).
  - Conventional Commits are required and validated by commitlint.
- Branching strategy: trunk-based development with short-lived branches.
  - The project may shift to GitFlow in the future if it becomes necessary.

## Domain Context
- MiLyst focuses on work tracking with a board-first UI.
- Core concepts (expected):
  - Tenants (organizations/workspaces)
  - Users and membership within a tenant
  - Boards, columns/statuses, tasks, ordering, and progress tracking
- **TBD**: collaboration model, permissions/roles, notifications, and audit/history.

## Important Constraints
- Keep the development workflow simple and predictable:
  - `task dev` / `task build` should remain the happy path.
- Multi-tenancy and auth are first-class concerns; avoid introducing patterns that make tenant isolation or security audits difficult.
- Prefer incremental architecture evolution; add complexity only when requirements justify it.

## External Dependencies
- Database: PostgreSQL
- Hosting targets: Azure and container-based hosting (specific deployment topology **TBD**)
